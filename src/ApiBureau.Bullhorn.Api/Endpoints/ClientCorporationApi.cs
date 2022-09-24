namespace ApiBureau.Bullhorn.Api.Endpoints;

public class ClientCorporationApi
{
    private readonly BullhornClient _bullhornApi;

    public ClientCorporationApi(BullhornClient bullhornApi) => _bullhornApi = bullhornApi;

    public async Task<List<ClientCorporationDto>> GetNewClientCorporationsAsync(long timestampFrom)
    {
        var query = $"ClientCorporation?fields=id,name&where=dateAdded>{timestampFrom}";

        return await _bullhornApi.QueryAsync<ClientCorporationDto>(query);
    }

    public async Task<List<ClientCorporationDto>> GetUpdatedClientCorporationsAsync(long timestampFrom)
    {
        var query = $"ClientCorporation?fields=id&where=dateLastModified>{timestampFrom}";

        return await _bullhornApi.QueryAsync<ClientCorporationDto>(query);
    }
}
