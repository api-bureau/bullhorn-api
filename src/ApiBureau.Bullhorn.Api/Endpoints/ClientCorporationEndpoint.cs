namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// Provides query and mutation operations for Bullhorn client corporations.
/// </summary>
public sealed class ClientCorporationEndpoint : QueryEndpointBase<ClientCorporationDto>
{
    private const string EntityDefaultFields = "id,name,dateAdded";

    internal ClientCorporationEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Creates a new client corporation.
    /// </summary>
    /// <param name="dto">The client corporation payload to create.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the create request.</returns>
    public Task<Result<ChangeResponse, ErrorResponse>> AddAsync(ClientCorporationDto dto, CancellationToken cancellationToken)
        => HttpClient.PutAsJsonAsync(EntityType.ClientCorporation, dto, cancellationToken);

    /// <summary>
    /// Updates an existing client corporation.
    /// </summary>
    /// <param name="clientCorporationId">The Bullhorn client corporation identifier.</param>
    /// <param name="data">The fields and values to update.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the update request.</returns>
    public Task<Result<ChangeResponse, ErrorResponse>> UpdateAsync(int clientCorporationId, object data, CancellationToken cancellationToken)
        => HttpClient.PostAsJsonAsync(EntityType.ClientCorporation, clientCorporationId, data, cancellationToken);
}