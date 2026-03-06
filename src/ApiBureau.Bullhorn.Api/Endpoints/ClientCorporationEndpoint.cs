namespace ApiBureau.Bullhorn.Api.Endpoints;

public class ClientCorporationEndpoint : QueryEndpointBase<ClientCorporationDto>
{
    private const string EntityDefaultFields = "id,name,dateAdded";

    public ClientCorporationEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    public Task<Result<ChangeResponse>> AddAsync(ClientCorporationDto dto, CancellationToken token)
        => ApiConnection.PutAsJsonAsync(EntityType.ClientCorporation, dto, token);

    /// <summary>
    /// Http POST /entity/ClientCorporation/{clientCorporationId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int clientCorporationId, object data, CancellationToken token)
        => ApiConnection.PostAsJsonAsync(EntityType.ClientCorporation, clientCorporationId, data, token);
}