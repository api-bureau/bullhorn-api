namespace ApiBureau.Bullhorn.Api.Endpoints;

public class PlacementCommissionEndpoint : QueryBaseEndpoint<PlacementCommissionDto>
{
    private const string EntityDefaultFields = "id,commissionPercentage,dateAdded,dateLastModified,placement(id),user(id),status,role";

    public PlacementCommissionEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    public Task<Result<ChangeResponse>> AddAsync(object content)
        => ApiConnection.PutAsJsonAsync(EntityType.PlacementCommission, content);

    /// <summary>
    /// This will soft delete the commission.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> DeleteAsync(int id, CancellationToken token)
        => ApiConnection.DeleteAsync(id, EntityType.PlacementCommission, token);
}