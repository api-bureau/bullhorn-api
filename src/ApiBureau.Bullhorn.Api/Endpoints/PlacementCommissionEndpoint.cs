namespace ApiBureau.Bullhorn.Api.Endpoints;

public class PlacementCommissionEndpoint : QueryBaseEndpoint<PlacementCommissionDto>
{
    private const string EntityDefaultFields = "id,commissionPercentage,dateAdded,dateLastModified,placement(id),user(id),status,role";

    public PlacementCommissionEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }
}