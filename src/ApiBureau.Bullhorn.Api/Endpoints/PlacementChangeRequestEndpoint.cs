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
    /// <param name="token">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the create request.</returns>
    public Task<Result<ChangeResponse>> AddAsync(object content, CancellationToken token)
        => HttpClient.PutAsJsonAsync(EntityType.PlacementChangeRequest, content, token);

    /// <summary>
    /// Updates an existing placement change request.
    /// </summary>
    /// <param name="placementChangeRequestId">The Bullhorn placement change request identifier.</param>
    /// <param name="data">The fields and values to update.</param>
    /// <param name="token">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the update request.</returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int placementChangeRequestId, object data, CancellationToken token)
        => HttpClient.PostAsJsonAsync(EntityType.PlacementChangeRequest, placementChangeRequestId, data, token);
}