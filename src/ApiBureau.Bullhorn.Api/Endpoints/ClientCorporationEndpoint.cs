namespace ApiBureau.Bullhorn.Api.Endpoints;

public class ClientCorporationEndpoint : QueryBaseEndpoint<ClientCorporationDto>
{
    private const string EntityDefaultFields = "id,name,dateAdded";

    public ClientCorporationEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    public Task<Result<ChangeResponse>> AddAsync(ClientCorporationDto dto)
        => ApiConnection.PutAsJsonAsync(EntityType.ClientCorporation, dto);

    /// <summary>
    /// Http POST /entity/ClientCorporation/{clientCorporationId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int clientCorporationId, object data)
        => ApiConnection.PostAsJsonAsync(EntityType.ClientCorporation, clientCorporationId, data);
}