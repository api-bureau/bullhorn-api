namespace ApiBureau.Bullhorn.Api.Endpoints;

public class MassUpdateEndpoint : BaseEndpoint
{
    public MassUpdateEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl) { }

    public Task<HttpResponseMessage> UpdateAsync(EntityType type, object data)
    {
        return ApiConnection.PostAsJsonAsync($"/{RequestUrl}/{type}", data);
    }
}