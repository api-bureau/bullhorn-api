using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Http.Json;

namespace ApiBureau.Bullhorn.Api.Http;

// Performs authentication exchanges only. The session manager owns cached credentials.
internal sealed class ApiSession(HttpClient client, BullhornSettings settings)
{
    internal async Task<TokenResponse> AuthorizeAsync(IProgress<string>? progress, CancellationToken token)
    {
        var request = new AuthorizationCodeRequest
        {
            Address = settings.AuthorizeUrl,
            ClientId = settings.ClientId,
            UserName = settings.UserName,
            Password = settings.Password
        };
        request.AddParameter("state", "ips");
        var authorization = await client.RequestAuthorizationCodeAsync(request, token);
        using var response = authorization.HttpResponse
            ?? throw new HttpRequestException("Bullhorn authorization returned no response.");
        response.EnsureSuccessStatusCode();
        var query = response.Headers.Location?.Query ?? response.RequestMessage?.RequestUri?.Query ?? "";
        var values = QueryHelpers.ParseQuery(query);
        if (!values.TryGetValue(settings.AuthorizationParameter, out var code) || string.IsNullOrWhiteSpace(code))
            throw new HttpRequestException("Bullhorn authorization returned no authorization code.");

        progress?.Report("Bullhorn authorization code received.");
        var exchange = new AuthorizationCodeTokenRequest
        {
            Address = settings.TokenUrl,
            ClientId = settings.ClientId,
            ClientSecret = settings.Secret,
            GrantType = "authorization_code"
        };
        exchange.AddParameter("code", code.ToString());
        var tokens = await client.RequestTokenAsync(exchange, token);
        using var tokenResponse = tokens.HttpResponse;
        return ValidateTokens(tokens);
    }

    // Null means this grant was rejected and full authorization is required.
    internal async Task<TokenResponse?> RefreshAsync(string refreshToken, CancellationToken token)
    {
        var tokens = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
        {
            Address = settings.TokenUrl,
            ClientId = settings.ClientId,
            ClientSecret = settings.Secret,
            RefreshToken = refreshToken
        }, token);
        using var response = tokens.HttpResponse;
        if (response?.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized &&
            string.Equals(tokens.Error, "invalid_grant", StringComparison.OrdinalIgnoreCase))
            return null;
        return ValidateTokens(tokens);
    }

    internal async Task<LoginResponse> LoginAsync(string accessToken, CancellationToken token)
    {
        var url = QueryHelpers.AddQueryString(settings.LoginUrl, new Dictionary<string, string?>
        {
            ["version"] = "2.0",
            ["access_token"] = accessToken,
            ["ttl"] = "240"
        });

        // Retry only REST login, with the same access token, after an explicit transient response.
        // Never repeat a single-use OAuth exchange or retry an ambiguous timeout.
        for (var attempt = 0; ; attempt++)
        {
            using var response = await client.GetAsync(url, token);
            if (attempt == 0 && response.StatusCode is HttpStatusCode.BadGateway
                or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), token);
                continue;
            }
            if (!response.IsSuccessStatusCode)
            {
                var error = await BullhornResponseReader.ReadErrorAsync(response, token);
                throw new HttpRequestException($"Bullhorn REST login failed: {error.Message}", null, response.StatusCode);
            }
            var login = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: token);
            if (string.IsNullOrWhiteSpace(login?.BhRestToken) || string.IsNullOrWhiteSpace(login.RestUrl))
                throw new HttpRequestException("Bullhorn REST login returned an invalid session.");
            return login;
        }
    }

    private static TokenResponse ValidateTokens(TokenResponse tokens)
    {
        if (tokens.IsError || string.IsNullOrWhiteSpace(tokens.AccessToken))
            throw new HttpRequestException(
                $"Bullhorn OAuth exchange failed: {tokens.ErrorDescription ?? tokens.Error ?? "Missing access token."}",
                null, tokens.HttpResponse?.StatusCode);
        return tokens;
    }
}