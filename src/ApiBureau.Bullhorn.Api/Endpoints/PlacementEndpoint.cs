namespace ApiBureau.Bullhorn.Api.Endpoints;

public class PlacementEndpoint : BaseEndpoint
{
    public PlacementEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    public async Task<List<PlacementDto>> GetFromAsync(long timestampFrom)
    {
        var query = $"Placement?fields=id,billingClientContact,candidate,dateAdded,dateLastModified,dateBegin,dateEnd,employeeType,employmentType,fee,flatFee,jobOrder,payRate,correlatedCustomText1,salary,salaryUnit,status&where=dateAdded>{timestampFrom}";

        return await ApiConnection.QueryAsync<PlacementDto>(query);
    }

    public async Task<List<PlacementDto>> GetNewAndUpdatedFromAsync(long timestampFrom)
    {
        var query = $"Placement?fields=id,billingClientContact,candidate,dateAdded,dateLastModified,dateBegin,dateEnd,employeeType,employmentType,fee,flatFee,jobOrder,payRate,correlatedCustomText1,salary,salaryUnit,status&where=dateAdded>{timestampFrom} OR dateLastModified>{timestampFrom}";

        return await ApiConnection.QueryAsync<PlacementDto>(query);
    }

    public Task<HttpResponseMessage> ApproveAsync(int placementId)
        => ApiConnection.PostAsync($"services/Placement/approve/{placementId}", null);
}