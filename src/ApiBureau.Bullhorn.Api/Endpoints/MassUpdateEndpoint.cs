using ApiBureau.Bullhorn.Api.Endpoints.Entities;

namespace ApiBureau.Bullhorn.Api.Endpoints;

public class MassUpdateEndpoint : BaseEndpoint
{
    public MassUpdateEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    public async Task<HttpResponseMessage> UpdateAsync(EntityType type, object data)
    {
        return await ApiConnection.PostAsJsonAsync($"/massUpdate/{type}", data);
    }
}
