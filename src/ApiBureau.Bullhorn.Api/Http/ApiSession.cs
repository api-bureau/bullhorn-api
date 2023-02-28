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
    private const int SessionRetry = 3;
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

    public async Task ConnectAsync()
    {
        for (var tryCount = 1; tryCount <= SessionRetry; tryCount++)
            try
            {
                var authorisationCode = await GetAuthorizationCodeAsync();

                var tokenResponse = await GetTokenResponseAsync(authorisationCode);

                await LoginAsync(tokenResponse);

                break;
            }
            catch (Exception e)
            {
                if (tryCount < SessionRetry)
                {
                    _logger.LogError(e, $"Session creation attempt {tryCount}/{SessionRetry}");
                    continue;
                }

                _logger.LogError(e, $"Session creation failed {tryCount}/{SessionRetry}");

                throw;
            }
    }

    private async Task<string> GetAuthorizationCodeAsync()
    {
        var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = _settings.AuthorizeUrl,
            ClientId = _settings.ClientId,
            ClientSecret = _settings.Secret,
            UserName = _settings.UserName,
            Password = _settings.Password,
            Parameters = {
                {"response_type", "code" },
                {"action", "Login" },
                {"state", "ips" }
            }
        });

        var collection = QueryHelpers.ParseQuery(GetQuery(response.HttpResponse));

        collection.TryGetValue(_settings.AuthorizationParameter, out var code);

        if (string.IsNullOrWhiteSpace(code))
        {
            _logger.LogError(NoAuthorizationCodeRetrieved);

            throw new Exception(NoAuthorizationCodeRetrieved);
        }

        return code;
    }

    private async Task<TokenResponse> GetTokenResponseAsync(string authorisationCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(authorisationCode);

        return await _client.RequestTokenAsync(new AuthorizationCodeTokenRequest
        {
            Address = _settings.TokenUrl,
            ClientId = _settings.ClientId,
            ClientSecret = _settings.Secret,
            GrantType = "authorization_code",
            Parameters = { { "code", authorisationCode } }
        });
    }

    private async Task LoginAsync(TokenResponse token)
    {
        ArgumentNullException.ThrowIfNull(token);

        var loginUrl = _settings.LoginUrl + $"?version=2.0&access_token={token.AccessToken}&ttl={SessionLength}";

        using var response = await _client.GetAsync(loginUrl);

        LoginResponse = await response.DeserializeAsync<LoginResponse>(_logger);

        if (LoginResponse == null || !LoginResponse.IsValid) throw new Exception("Login failed, LoginResponse is null.");

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

    private async Task<TokenResponse> GetRefreshTokenAsync() =>
        await _client.RequestRefreshTokenAsync(new RefreshTokenRequest
        {
            Address = _settings.TokenUrl,
            ClientId = _settings.ClientId,
            ClientSecret = _settings.Secret,
            RefreshToken = _refreshToken
        });

    private static string GetQuery(HttpResponseMessage response) =>
        response.Headers?.Location?.Query ?? response.RequestMessage.RequestUri.Query;
}