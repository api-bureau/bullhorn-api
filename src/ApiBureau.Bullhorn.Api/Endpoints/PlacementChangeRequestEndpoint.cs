namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// Provides query and mutation operations for Bullhorn placement change requests.
/// </summary>
public sealed class PlacementChangeRequestEndpoint : QueryEndpointBase<PlacementChangeRequestDto>
{
    private const string EntityDefaultFields = "id,dateAdded,dateLastModified,status,title,requestStatus,requestType,placement(id),customText12";

    internal PlacementChangeRequestEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Creates a new placement change request.
    /// </summary>
    /// <param name="content">The placement change request payload to create.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the create request.</returns>
    public Task<Result<ChangeResponse>> AddAsync(object content, CancellationToken cancellationToken)
        => HttpClient.PutAsJsonAsync(EntityType.PlacementChangeRequest, content, cancellationToken);

    /// <summary>
    /// Updates an existing placement change request.
    /// </summary>
    /// <param name="placementChangeRequestId">The Bullhorn placement change request identifier.</param>
    /// <param name="data">The fields and values to update.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the update request.</returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int placementChangeRequestId, object data, CancellationToken cancellationToken)
        => HttpClient.PostAsJsonAsync(EntityType.PlacementChangeRequest, placementChangeRequestId, data, cancellationToken);
}