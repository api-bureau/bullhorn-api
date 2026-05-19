
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace ApiBureau.Bullhorn.Api.Http;

public class ApiSession
{
    private readonly HttpClient _client;
    private readonly ILogger _logger;
    private readonly BullhornSettings _settings;
    private const int SessionLength = 240; // max 240;
    private const int SessionRetry = 5;
    private const int DelayBetweenRetriesInMs = 200;
    private const string NoAuthorizationCodeRetrieved = "No authorization code retrieved.";
    private const string AuthorizationState = "ips";
    private string? _refreshToken;

    public LoginResponse? LoginResponse { get; private set; }
    public PingResponse Ping { get; set; } = new PingResponse();
    public bool IsValid => LoginResponse != null && LoginResponse.IsValid;

    public ApiSession(HttpClient client, BullhornSettings settings, ILogger logger)
    {
        _client = client;
        _logger = logger;
        _settings = settings;
    }

    public async Task ConnectAsync(IProgress<string>? progress = null, CancellationToken token = default)
        => await ExecuteWithRetryAsync(
            async () =>
            {
                var authorisationCode = await GetAuthorizationCodeAsync(progress, token);
                var tokenResponse = await GetTokenResponseAsync(authorisationCode, progress, token);

                await LoginAsync(tokenResponse, progress, token);

                ReportProgress(progress, "Connection successful");
            },
            tryCount => $"Attempt {tryCount}/{SessionRetry}: Trying to connect...",
            (tryCount, exception) => $"Attempt {tryCount}/{SessionRetry} failed. Reason: {exception.Message}",
            progress,
            token);

    private async Task<string> GetAuthorizationCodeAsync(IProgress<string>? progress = null, CancellationToken token = default)
    {
        var request = new AuthorizationCodeRequest
        {
            Address = _settings.AuthorizeUrl,
            ClientId = _settings.ClientId,
            UserName = _settings.UserName,
            Password = _settings.Password
        };
        request.AddParameter("state", AuthorizationState);

        var response = await _client.RequestAuthorizationCodeAsync(request, token);

        if (response.HttpResponse is null)
        {
            ThrowInvalidOperation("No response received", response.ErrorDescription ?? response.Error);
        }

        var collection = QueryHelpers.ParseQuery(GetQuery(response.HttpResponse));

        collection.TryGetValue(_settings.AuthorizationParameter, out var code);

        if (string.IsNullOrWhiteSpace(code))
        {
            ThrowInvalidOperation(NoAuthorizationCodeRetrieved);
        }

        ReportProgress(progress, "Authorization was successful");

        return code!;
    }

    private async Task<TokenResponse> GetTokenResponseAsync(string authorisationCode, IProgress<string>? progress = null, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(authorisationCode);

        var request = new AuthorizationCodeTokenRequest
        {
            Address = _settings.TokenUrl,
            ClientId = _settings.ClientId,
            ClientSecret = _settings.Secret,
            GrantType = "authorization_code"
        };
        request.AddParameter("code", authorisationCode);

        var response = await _client.RequestTokenAsync(request, token);

        var validatedResponse = EnsureTokenResponse(response, "Error retrieving token");

        ReportProgress(progress, "Token retrieval was successful");

        return validatedResponse;
    }

    // This API call is failing in some cases, so we retry it a few times through ExecuteWithRetryAsync
    //{"errorMessage":"Invalid or expired OAuth access token.","errorMessageKey":"errors.authentication.invalidOAuthToken","errorCode":400}
    private async Task LoginAsync(TokenResponse token, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);

        ArgumentException.ThrowIfNullOrEmpty(token.AccessToken);

        var loginUrl = BuildLoginUrl(token.AccessToken);

        using var response = await _client.GetAsync(loginUrl, cancellationToken);

        // temp variable to log the response in case of failure
        //var temp = await response.Content.ReadAsStringAsync();

        var loginResponse = await response.DeserializeAsync<LoginResponse>(_logger);
        EnsureLoginResponse(loginResponse);

        LoginResponse = loginResponse;
        UpdateBhRestTokenHeader(loginResponse?.BhRestToken ?? throw new InvalidOperationException("Login failed, BhRestToken is null."));

        _refreshToken = token.RefreshToken;

        Ping.SetExpiryDate(DateTime.UtcNow.AddMinutes(SessionLength).Timestamp());

        ReportProgress(progress, "Login was successful");
    }

    private void UpdateBhRestTokenHeader(string token)
    {
        _client.DefaultRequestHeaders.Remove("BhRestToken");
        _client.DefaultRequestHeaders.TryAddWithoutValidation("BhRestToken", token);
    }

    public async Task RefreshTokenAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(_refreshToken);

        await ExecuteWithRetryAsync(
            async () =>
            {
                var tokenResponse = await GetRefreshTokenAsync(cancellationToken);
                await LoginAsync(tokenResponse, cancellationToken: cancellationToken);
            },
            tryCount => $"Refresh session creation attempt {tryCount}/{SessionRetry}",
            (tryCount, exception) => $"Refresh session creation attempt {tryCount}/{SessionRetry} failed. Reason: {exception.Message}",
            cancellationToken: cancellationToken);
    }

    private async Task<TokenResponse> GetRefreshTokenAsync(CancellationToken token)
    {
        var response = await _client.RequestRefreshTokenAsync(new RefreshTokenRequest
        {
            Address = _settings.TokenUrl,
            ClientId = _settings.ClientId,
            ClientSecret = _settings.Secret,
            RefreshToken = _refreshToken!
        }, token);

        return EnsureTokenResponse(response, "Error refreshing token");
    }

    private static string GetQuery(HttpResponseMessage response) =>
        response.Headers?.Location?.Query ?? response.RequestMessage?.RequestUri?.Query ?? "";

    private static void ReportProgress(IProgress<string>? progress, string message)
        => progress?.Report(message);

    private string BuildLoginUrl(string accessToken) =>
        QueryHelpers.AddQueryString(_settings.LoginUrl, new Dictionary<string, string?>
        {
            ["version"] = "2.0",
            ["access_token"] = accessToken,
            ["ttl"] = SessionLength.ToString()
        });

    private void EnsureLoginResponse(LoginResponse? loginResponse)
    {
        if (loginResponse is null)
        {
            ThrowInvalidOperation("Login failed, LoginResponse is null");
        }

        if (!loginResponse.Success)
        {
            ThrowInvalidOperation("Login failed", $"{loginResponse.ErrorMessageKey}, {loginResponse.ErrorMessage}");
        }

        if (!loginResponse.IsValid)
        {
            ThrowInvalidOperation("Login failed, invalid BhRestToken.");
        }
    }

    private TokenResponse EnsureTokenResponse(TokenResponse? response, string customMessage)
    {
        if (response is null || response.IsError)
        {
            ThrowInvalidOperation(customMessage, response?.ErrorDescription ?? response?.Error);
        }

        return response;
    }

    private async Task ExecuteWithRetryAsync(
        Func<Task> action,
        Func<int, string>? attemptMessageFactory = null,
        Func<int, Exception, string>? failureMessageFactory = null,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        for (var tryCount = 1; tryCount <= SessionRetry; tryCount++)
        {
            try
            {
                if (attemptMessageFactory is not null)
                {
                    ReportProgress(progress, attemptMessageFactory(tryCount));
                }

                await action();
                return;
            }
            catch (Exception exception) when (tryCount < SessionRetry)
            {
                var failureMessage = failureMessageFactory?.Invoke(tryCount, exception)
                    ?? $"Attempt {tryCount}/{SessionRetry} failed. Reason: {exception.Message}";

                ReportProgress(progress, $"{failureMessage}. Retrying...");
                _logger.LogError(exception, "{failureMessage}. Retrying...", failureMessage);

                await Task.Delay(DelayBetweenRetriesInMs * tryCount, cancellationToken);
            }
            catch (Exception exception)
            {
                var failureMessage = failureMessageFactory?.Invoke(tryCount, exception)
                    ?? $"Attempt {tryCount}/{SessionRetry} failed. Reason: {exception.Message}";

                ReportProgress(progress, $"{failureMessage}. No more retries.");
                _logger.LogError(exception, "{failureMessage}. No more retries.", failureMessage);

                throw;
            }
        }
    }

    [DoesNotReturn]
    private void ThrowInvalidOperation(string customMessage, string? responseMessage = null)
    {
        if (string.IsNullOrWhiteSpace(responseMessage))
        {
            throw new InvalidOperationException($"An error occurred: {customMessage}");
        }
        else
        {
            throw new InvalidOperationException($"An error occurred: {responseMessage}, {customMessage}");
        }
    }
}