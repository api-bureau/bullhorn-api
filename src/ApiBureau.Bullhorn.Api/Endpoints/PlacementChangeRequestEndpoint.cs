namespace ApiBureau.Bullhorn.Api.Endpoints;

public class PlacementChangeRequestEndpoint : QueryEndpointBase<PlacementChangeRequestDto>
{
    private const string EntityDefaultFields = "id,dateAdded,dateLastModified,status,title,requestStatus,requestType,placement(id),customText12";

    public PlacementChangeRequestEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    public Task<Result<ChangeResponse>> AddAsync(object content)
        => ApiConnection.PutAsJsonAsync(EntityType.PlacementChangeRequest, content);

    public Task<Result<ChangeResponse>> UpdateAsync(int placementChangeRequestId, object data, CancellationToken token)
        => ApiConnection.PostAsJsonAsync(EntityType.PlacementChangeRequest, placementChangeRequestId, data, token);
}