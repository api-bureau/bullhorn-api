namespace ApiBureau.Bullhorn.Api.Endpoints;

public class PlacementCommissionApi : BaseEndpoint
{
    public PlacementCommissionApi(ApiConnection apiConnection) : base(apiConnection) { }

    public async Task<List<PlacementCommissionDto>> GetFromAsync(long timestampFrom)
    {
        var query = $"PlacementCommission?fields=id,commissionPercentage,dateAdded,dateLastModified,placement(id),user(id),status,role&where=dateAdded>{timestampFrom}";

        return await ApiConnection.QueryAsync<PlacementCommissionDto>(query);
    }

    public async Task<List<PlacementCommissionDto>> GetNewAndUpdatedFromAsync(long timestampFrom)
    {
        var query = $"PlacementCommission?fields=id,commissionPercentage,dateAdded,dateLastModified,placement(id),user(id),status,role&where=dateAdded>{timestampFrom} OR dateLastModified>{timestampFrom}";

        return await ApiConnection.QueryAsync<PlacementCommissionDto>(query);
    }
}
