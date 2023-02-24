namespace ApiBureau.Bullhorn.Api.Endpoints;

public class PlacementEndpoint : QueryBaseEndpoint<PlacementDto>
{
    private const string EntityDefaultFields = "id,billingClientContact,candidate,dateAdded,dateLastModified,dateBegin,dateEnd,employeeType,employmentType,fee,flatFee,jobOrder,payRate,correlatedCustomText1,salary,salaryUnit,status";

    public PlacementEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    public Task<HttpResponseMessage> ApproveAsync(int placementId)
        => ApiConnection.PostAsync($"services/{RequestUrl}/approve/{placementId}", null);

    /// <summary>
    /// Http POST /entity/Placement/{placementId}
    /// </summary>
    /// <returns></returns>
    public Task<HttpResponseMessage> UpdateAsync(int placementId, object data)
        => ApiConnection.PostAsJsonAsync(EntityType.Placement, placementId, data);
}