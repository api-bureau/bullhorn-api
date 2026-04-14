namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// Provides query and mutation operations for Bullhorn client corporations.
/// </summary>
public class ClientCorporationEndpoint : QueryEndpointBase<ClientCorporationDto>
{
    private const string EntityDefaultFields = "id,name,dateAdded";

    public ClientCorporationEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Creates a new client corporation.
    /// </summary>
    /// <param name="dto">The client corporation payload to create.</param>
    /// <param name="token">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the create request.</returns>
    public Task<Result<ChangeResponse>> AddAsync(ClientCorporationDto dto, CancellationToken token)
        => ApiConnection.PutAsJsonAsync(EntityType.ClientCorporation, dto, token);

    /// <summary>
    /// Updates an existing client corporation.
    /// </summary>
    /// <param name="clientCorporationId">The Bullhorn client corporation identifier.</param>
    /// <param name="data">The fields and values to update.</param>
    /// <param name="token">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the update request.</returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int clientCorporationId, object data, CancellationToken token)
        => ApiConnection.PostAsJsonAsync(EntityType.ClientCorporation, clientCorporationId, data, token);
}