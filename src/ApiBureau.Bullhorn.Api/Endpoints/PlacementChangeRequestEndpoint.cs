namespace ApiBureau.Bullhorn.Api.Endpoints;

public class PlacementChangeRequestEndpoint : BaseEndpoint
{
    public PlacementChangeRequestEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    public async Task<List<PlacementChangeRequestDto>> GetNewAndUpdatedFromAsync(long timestampFrom)
    {
        var query = $"PlacementChangeRequest?fields=id,dateAdded,dateLastModified,requestStatus,requestType,placement(id),customText12&where=dateAdded>{timestampFrom} OR dateLastModified>{timestampFrom}";

        return await ApiConnection.QueryAsync<PlacementChangeRequestDto>(query);
    }

    public Task<HttpResponseMessage> AddAsync(object content)
        => ApiConnection.PutAsJsonAsync(EntityType.PlacementChangeRequest, content);
}
