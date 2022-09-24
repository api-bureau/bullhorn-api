namespace ApiBureau.Bullhorn.Api.Endpoints;

public class ClientCorporationEndpoint : BaseEndpoint
{
    public ClientCorporationEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    public Task<HttpResponseMessage> AddAsync(ClientCorporationDto dto)
        => ApiConnection.PutAsJsonAsync(EntityType.ClientCorporation, dto);

    public async Task<List<ClientCorporationDto>> GetNewClientCorporationsAsync(long timestampFrom)
    {
        var query = $"ClientCorporation?fields=id,name&where=dateAdded>{timestampFrom}";

        return await ApiConnection.QueryAsync<ClientCorporationDto>(query);
    }

    public async Task<List<ClientCorporationDto>> GetUpdatedClientCorporationsAsync(long timestampFrom)
    {
        var query = $"ClientCorporation?fields=id&where=dateLastModified>{timestampFrom}";

        return await ApiConnection.QueryAsync<ClientCorporationDto>(query);
    }

    /// <summary>
    /// Http POST /entity/ClientCorporation/{clientCorporationId}
    /// </summary>
    /// <returns></returns>
    public Task<HttpResponseMessage> UpdateAsync(int clientCorporationId, object data)
        => ApiConnection.PostAsJsonAsync(EntityType.ClientCorporation, clientCorporationId, data);
}
