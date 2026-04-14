namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// Provides query and mutation operations for Bullhorn placement commissions.
/// </summary>
public class PlacementCommissionEndpoint : QueryEndpointBase<PlacementCommissionDto>
{
    private const string EntityDefaultFields = "id,commissionPercentage,dateAdded,dateLastModified,placement(id),user(id),status,role";

    public PlacementCommissionEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Creates a new placement commission.
    /// </summary>
    /// <param name="content">The placement commission payload to create.</param>
    /// <param name="token">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the create request.</returns>
    public Task<Result<ChangeResponse>> AddAsync(object content, CancellationToken token)
        => ApiConnection.PutAsJsonAsync(EntityType.PlacementCommission, content, token);

    /// <summary>
    /// Soft deletes the specified placement commission.
    /// </summary>
    /// <param name="id">The Bullhorn placement commission identifier.</param>
    /// <param name="token">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the delete request.</returns>
    public Task<Result<ChangeResponse>> DeleteAsync(int id, CancellationToken token)
        => ApiConnection.DeleteAsync(id, EntityType.PlacementCommission, token);
}