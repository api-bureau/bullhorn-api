using IdentityModel.Client;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

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

    public async Task ConnectAsync(IProgress<string>? progress = null)
    {
        for (var tryCount = 1; tryCount <= SessionRetry; tryCount++)
        {
            try
            {
                progress?.Report($"Attempt {tryCount}/{SessionRetry}: Trying to connect...");

                var authorisationCode = await GetAuthorizationCodeAsync();

                if (authorisationCode != null)
                {
                    progress?.Report("Authorisation was successful");
                }

                var tokenResponse = await GetTokenResponseAsync(authorisationCode);

                if (!tokenResponse.IsError)
                {
                    progress?.Report("Token retrieval was successful");
                }

                await LoginAsync(tokenResponse);

                progress?.Report("Connection successful!");

                break;
            }
            catch (Exception e)
            {
                if (tryCount < SessionRetry)
                {
                    progress?.Report($"Attempt {tryCount}/{SessionRetry} failed. Retrying...");

                    _logger.LogError(e, $"Session creation attempt {tryCount}/{SessionRetry}");

                    await Task.Delay(DelayBetweenRetriesInMs * tryCount);

                    continue;
                }

                progress?.Report($"Attempt {tryCount}/{SessionRetry} failed. No more retries.");

                _logger.LogError(e, $"Session creation failed {tryCount}/{SessionRetry}");

                throw;
            }
        }
    }

    private async Task<string?> GetAuthorizationCodeAsync()
    {
        var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = _settings.AuthorizeUrl,
            ClientId = _settings.ClientId,
            ClientSecret = _settings.Secret,
            UserName = _settings.UserName,
            Password = _settings.Password,
            ClientCredentialStyle = ClientCredentialStyle.PostBody,
            Parameters = {
                {"response_type", "code" },
                {"action", "Login" },
                {"state", "ips" },
            }
        });

        var collection = QueryHelpers.ParseQuery(GetQuery(response.HttpResponse));

        collection.TryGetValue(_settings.AuthorizationParameter, out var code);

        if (string.IsNullOrWhiteSpace(code))
        {
            _logger.LogError(NoAuthorizationCodeRetrieved);

            throw new InvalidOperationException(NoAuthorizationCodeRetrieved);
        }

        return code;
    }

    private async Task<TokenResponse> GetTokenResponseAsync(string? authorisationCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(authorisationCode);

        return await _client.RequestTokenAsync(new AuthorizationCodeTokenRequest
        {
            Address = _settings.TokenUrl,
            ClientId = _settings.ClientId,
            ClientSecret = _settings.Secret,
            GrantType = "authorization_code",
            Parameters = { { "code", authorisationCode } },
            ClientCredentialStyle = ClientCredentialStyle.PostBody
        });
    }

    // This API call is failing in some cases, so we retry it a few times
    //{"errorMessage":"Invalid or expired OAuth access token.","errorMessageKey":"errors.authentication.invalidOAuthToken","errorCode":400}
    private async Task LoginAsync(TokenResponse token)
    {
        ArgumentNullException.ThrowIfNull(token);

        var loginUrl = _settings.LoginUrl + $"?version=2.0&access_token={token.AccessToken}&ttl={SessionLength}";

        using var response = await _client.GetAsync(loginUrl);

        // temp variable to log the response in case of failure
        //var temp = await response.Content.ReadAsStringAsync();

        LoginResponse = await response.DeserializeAsync<LoginResponse>(_logger);

        if (LoginResponse is null)
        {
            throw new InvalidOperationException($"Login failed, LoginResponse is null");
        }

        if (!LoginResponse.Success)
        {
            throw new InvalidOperationException($"Login failed, error received: {LoginResponse.ErrorMessageKey}, {LoginResponse.ErrorMessage}");
        }

        if (!LoginResponse.IsValid)
        {
            throw new InvalidOperationException($"Login failed, Invalid BhRestToken.");
        }

        UpdateBhRestTokenHeader(LoginResponse.BhRestToken!);

        _refreshToken = token.RefreshToken;

        Ping.SetExpiryDate(DateTime.Now.AddMinutes(SessionLength).Timestamp());
    }

    private void UpdateBhRestTokenHeader(string token)
    {
        _client.DefaultRequestHeaders.Remove("BhRestToken");
        _client.DefaultRequestHeaders.TryAddWithoutValidation("BhRestToken", token);
    }

    public async Task RefreshTokenAsync()
    {
        ArgumentException.ThrowIfNullOrEmpty(_refreshToken);

        var tokenResponse = await GetRefreshTokenAsync();

        for (var tryCount = 1; tryCount <= SessionRetry; tryCount++)
        {
            try
            {
                await LoginAsync(tokenResponse);

                break;
            }
            catch (Exception e)
            {
                if (tryCount < SessionRetry)
                {
                    _logger.LogError(e, $"Refresh Session creation attempt {tryCount}/{SessionRetry}");
                    continue;
                }

                _logger.LogError(e, $"Refresh Session creation failed {tryCount}/{SessionRetry}");

                throw;
            }
        }
    }

    private async Task<TokenResponse> GetRefreshTokenAsync() =>
        await _client.RequestRefreshTokenAsync(new RefreshTokenRequest
        {
            Address = _settings.TokenUrl,
            ClientId = _settings.ClientId,
            ClientSecret = _settings.Secret,
            RefreshToken = _refreshToken!,
            ClientCredentialStyle = ClientCredentialStyle.PostBody
        });

    private static string GetQuery(HttpResponseMessage response) =>
        response.Headers?.Location?.Query ?? response.RequestMessage.RequestUri.Query;
}