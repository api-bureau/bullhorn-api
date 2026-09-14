namespace ApiBureau.Bullhorn.Api.Http;

// Each snapshot has reference identity so concurrent recovery can detect replacement.
internal sealed class BullhornSession
{
    internal BullhornSession(string token, string restUrl)
    {
        Token = token;
        RestUrl = restUrl;
    }

    internal string Token { get; }

    internal string RestUrl { get; }
}