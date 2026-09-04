namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// Provides query and update operations for Bullhorn opportunities.
/// </summary>
public sealed class OpportunityEndpoint : QueryEndpointBase<OpportunityDto>
{
    private const string EntityDefaultFields = "id,dateAdded,dateLastModified,status,title,source,owner,isOpen,isDeleted,clientContact,clientCorporation";

    internal OpportunityEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Updates an existing opportunity.
    /// </summary>
    /// <param name="opportunityId">The Bullhorn opportunity identifier.</param>
    /// <param name="data">The fields and values to update.</param>
    /// <param name="token">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the update request.</returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int opportunityId, object data, CancellationToken token)
        => HttpClient.PostAsJsonAsync(EntityType.Opportunity, opportunityId, data, token);
}