namespace ApiBureau.Bullhorn.Api.Endpoints;

public class PlacementEndpoint : QueryEndpointBase<PlacementDto>
{
    private const string EntityDefaultFields = "id,billingClientContact,candidate,dateAdded,dateLastModified,dateBegin,dateEnd,employeeType,employmentType,fee,flatFee,jobOrder,payRate,correlatedCustomText1,salary,salaryUnit,status";

    public PlacementEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    public Task<HttpResponseMessage> ApproveAsync(int placementId)
        => ApiConnection.PostAsync($"services/{RequestUrl}/approve/{placementId}", null);

    /// <summary>
    /// Http POST /entity/Placement/{placementId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int placementId, object data, CancellationToken token = default)
        => ApiConnection.PostAsJsonAsync(EntityType.Placement, placementId, data, token);

    public Task<Result<ChangeResponse>> AddAsync(object content)
        => ApiConnection.PutAsJsonAsync(EntityType.Placement, content);
}