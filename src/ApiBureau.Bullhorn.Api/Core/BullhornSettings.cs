namespace ApiBureau.Bullhorn.Api.Core;

public class BullhornSettings
{
    /// <summary>Maximum time to reuse a server-verified REST session before pinging again.</summary>
    public TimeSpan SessionVerificationInterval { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Needed for Authorisation
    /// </summary>
    public string AuthorizationParameter { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string Secret { get; set; } = default!;
    public string LoginUrl { get; set; } = default!;
    public string TokenUrl { get; set; } = default!;
    public string AuthorizeUrl { get; set; } = default!;
}