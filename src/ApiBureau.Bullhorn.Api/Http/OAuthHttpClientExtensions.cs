using System.Text.Json;

namespace ApiBureau.Bullhorn.Api.Http;

public static class OAuthHttpClientExtensions
{
    public static Task<TokenResponse> RequestAuthorizationCodeAsync(this HttpClient client, AuthorizationCodeRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(request);

        return SendAuthorizationRequestAsync(client, request, cancellationToken);
    }

    public static Task<TokenResponse> RequestTokenAsync(this HttpClient client, AuthorizationCodeTokenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(request);

        return SendTokenRequestAsync(client, request, cancellationToken);
    }

    public static Task<TokenResponse> RequestRefreshTokenAsync(this HttpClient client, RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(request);

        return SendTokenRequestAsync(client, request, cancellationToken);
    }

    private static async Task<TokenResponse> SendAuthorizationRequestAsync(HttpClient client, AuthorizationCodeRequest request, CancellationToken cancellationToken)
    {
        var requestUri = request.BuildRequestUri();
        using var message = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        return new TokenResponse
        {
            HttpResponse = response,
            IsError = !response.IsSuccessStatusCode
        };
    }

    private static async Task<TokenResponse> SendTokenRequestAsync(HttpClient client, OAuthRequestBase request, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, request.Address)
        {
            Content = new FormUrlEncodedContent(request.BuildFormValues())
        };

        var response = await client.SendAsync(message, cancellationToken);
        var tokenResponse = await CreateTokenResponseAsync(response, cancellationToken);
        tokenResponse.HttpResponse = response;

        return tokenResponse;
    }

    private static async Task<TokenResponse> CreateTokenResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        TokenResponse? tokenResponse = null;

        if (response.Content is not null)
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            if (stream.CanRead)
            {
                tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(stream, cancellationToken: cancellationToken);
            }
        }

        tokenResponse ??= new TokenResponse();
        tokenResponse.IsError = !response.IsSuccessStatusCode || !string.IsNullOrWhiteSpace(tokenResponse.Error);
        tokenResponse.Error ??= tokenResponse.ErrorDescription;

        if (tokenResponse.IsError && string.IsNullOrWhiteSpace(tokenResponse.Error))
        {
            tokenResponse.Error = response.ReasonPhrase;
        }

        return tokenResponse;
    }
}