using Microsoft.AspNetCore.WebUtilities;
using System.Collections.ObjectModel;

namespace ApiBureau.Bullhorn.Api.Http;

public abstract class OAuthRequestBase
{
    public string Address { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string? ClientSecret { get; set; }
    public Collection<KeyValuePair<string, string>> Parameters { get; } = [];

    public virtual IEnumerable<KeyValuePair<string, string>> BuildFormValues()
    {
        if (string.IsNullOrWhiteSpace(Address))
        {
            throw new InvalidOperationException("OAuth request address is required.");
        }

        if (string.IsNullOrWhiteSpace(ClientId))
        {
            throw new InvalidOperationException("OAuth client id is required.");
        }

        var values = new List<KeyValuePair<string, string>>
        {
            new("client_id", ClientId)
        };

        if (!string.IsNullOrWhiteSpace(ClientSecret))
        {
            values.Add(new("client_secret", ClientSecret));
        }

        values.AddRange(GetGrantSpecificValues());
        values.AddRange(Parameters);

        return values;
    }

    protected abstract IEnumerable<KeyValuePair<string, string>> GetGrantSpecificValues();

    public void AddParameter(string key, string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(value);

        Parameters.Add(new(key, value));
    }

    protected IEnumerable<KeyValuePair<string, string?>> GetNullableParameters() =>
        Parameters.Select(parameter => new KeyValuePair<string, string?>(parameter.Key, parameter.Value));
}

public sealed class AuthorizationCodeRequest : OAuthRequestBase
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ResponseType { get; set; } = "code";
    public string Action { get; set; } = "Login";

    protected override IEnumerable<KeyValuePair<string, string>> GetGrantSpecificValues()
    {
        if (string.IsNullOrWhiteSpace(UserName))
        {
            throw new InvalidOperationException("OAuth username is required.");
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            throw new InvalidOperationException("OAuth password is required.");
        }

        if (string.IsNullOrWhiteSpace(ResponseType))
        {
            throw new InvalidOperationException("OAuth response type is required.");
        }

        if (string.IsNullOrWhiteSpace(Action))
        {
            throw new InvalidOperationException("OAuth action is required.");
        }

        return
        [
            new("client_id", ClientId),
            new("response_type", ResponseType),
            new("action", Action),
            new("username", UserName),
            new("password", Password)
        ];
    }

    public Uri BuildRequestUri() => new(
        QueryHelpers.AddQueryString(
            Address,
            GetGrantSpecificValues().Select(parameter => new KeyValuePair<string, string?>(parameter.Key, parameter.Value))
                .Concat(GetNullableParameters())));
}

public sealed class AuthorizationCodeTokenRequest : OAuthRequestBase
{
    public string GrantType { get; set; } = "authorization_code";

    protected override IEnumerable<KeyValuePair<string, string>> GetGrantSpecificValues()
    {
        if (string.IsNullOrWhiteSpace(GrantType))
        {
            throw new InvalidOperationException("OAuth grant type is required.");
        }

        return [new("grant_type", GrantType)];
    }
}

public sealed class RefreshTokenRequest : OAuthRequestBase
{
    public string RefreshToken { get; set; } = string.Empty;

    protected override IEnumerable<KeyValuePair<string, string>> GetGrantSpecificValues()
    {
        if (string.IsNullOrWhiteSpace(RefreshToken))
        {
            throw new InvalidOperationException("OAuth refresh token is required.");
        }

        return
        [
            new("grant_type", "refresh_token"),
            new("refresh_token", RefreshToken)
        ];
    }
}

public sealed class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int? ExpiresIn { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; set; }

    [JsonIgnore]
    public bool IsError { get; set; }

    [JsonIgnore]
    public HttpResponseMessage? HttpResponse { get; set; }
}