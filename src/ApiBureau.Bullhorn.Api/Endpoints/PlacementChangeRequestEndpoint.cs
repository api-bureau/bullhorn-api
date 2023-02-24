namespace ApiBureau.Bullhorn.Api.Endpoints;

public class PlacementChangeRequestEndpoint : QueryBaseEndpoint<PlacementChangeRequestDto>
{
    private const string EntityDefaultFields = "id,dateAdded,dateLastModified,status,title,requestStatus,requestType,placement(id),customText12";

    public PlacementChangeRequestEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    public Task<HttpResponseMessage> AddAsync(object content)
        => ApiConnection.PutAsJsonAsync(EntityType.PlacementChangeRequest, content);
}