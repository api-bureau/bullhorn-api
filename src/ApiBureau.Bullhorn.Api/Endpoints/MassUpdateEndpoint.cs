namespace ApiBureau.Bullhorn.Api.Endpoints;

public class MassUpdateEndpoint : EndpointBase
{
    public MassUpdateEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl) { }

    public Task<HttpResponseMessage> UpdateAsync(EntityType type, object data, CancellationToken token)
        => ApiConnection.PostAsJsonAsync($"/{RequestUrl}/{type}", data, token);
}