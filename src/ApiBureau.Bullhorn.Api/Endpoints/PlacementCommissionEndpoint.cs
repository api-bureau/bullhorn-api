namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// Provides query and mutation operations for Bullhorn placement commissions.
/// </summary>
public sealed class PlacementCommissionEndpoint : QueryEndpointBase<PlacementCommissionDto>
{
    private const string EntityDefaultFields = "id,commissionPercentage,dateAdded,dateLastModified,placement(id),user(id),status,role";

    internal PlacementCommissionEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Creates a new placement commission.
    /// </summary>
    /// <param name="content">The placement commission payload to create.</param>
    /// <param name="token">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the create request.</returns>
    public Task<Result<ChangeResponse>> AddAsync(object content, CancellationToken token)
        => HttpClient.PutAsJsonAsync(EntityType.PlacementCommission, content, token);

    /// <summary>
    /// Soft deletes the specified placement commission.
    /// </summary>
    /// <param name="id">The Bullhorn placement commission identifier.</param>
    /// <param name="token">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the delete request.</returns>
    public Task<Result<ChangeResponse>> DeleteAsync(int id, CancellationToken token)
        => HttpClient.DeleteAsync(id, EntityType.PlacementCommission, token);
}