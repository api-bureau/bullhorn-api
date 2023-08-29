namespace ApiBureau.Bullhorn.Api.Endpoints;

public class OpportunityEndpoint : QueryBaseEndpoint<JobOrderDto>
{
    private const string EntityDefaultFields = "id,dateAdded,dateLastModified,status,title,source,owner,isOpen,isDeleted,clientContact,clientCorporation";

    public OpportunityEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Http POST /entity/Opportunity/{opportunityId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int opportunityId, object data)
        => ApiConnection.PostAsJsonAsync(EntityType.Opportunity, opportunityId, data);
}