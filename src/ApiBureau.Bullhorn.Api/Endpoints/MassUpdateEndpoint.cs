namespace ApiBureau.Bullhorn.Api.Endpoints;

public class MassUpdateEndpoint : BaseEndpoint
{
    public MassUpdateEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    public Task<HttpResponseMessage> UpdateAsync(EntityType type, object data)
    {
        return ApiConnection.PostAsJsonAsync($"/massUpdate/{type}", data);
    }
}