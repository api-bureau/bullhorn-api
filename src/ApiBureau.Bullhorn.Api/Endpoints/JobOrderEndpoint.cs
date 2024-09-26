namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// JobOrder entity with default fields: id,dateAdded,dateLastModified,status,title,source,owner,isOpen,isDeleted,clientContact,clientCorporation
/// </summary>
public class JobOrderEndpoint : QueryBaseEndpoint<JobOrderDto>
{
    private const string EntityDefaultFields = "id,dateAdded,dateLastModified,status,title,source,owner,isOpen,isDeleted,clientContact,clientCorporation";

    public JobOrderEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    [Obsolete("Use QueryNewAndUpdatedFromAsync", true)]
    public async Task<List<JobOrderDto>> GetNewAndUpdatedFromAsync(long timestampFrom)
    {
        var query = $"{RequestUrl}?fields={DefaultFields}&where=dateAdded>{timestampFrom} OR dateLastModified>{timestampFrom}";

        return await ApiConnection.QueryAsync<JobOrderDto>(query);
    }

    /// <summary>
    /// Http POST /entity/JobOrder/{jobOrderId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int jobOrderId, object data, CancellationToken token = default)
        => ApiConnection.PostAsJsonAsync(EntityType.JobOrder, jobOrderId, data, token);
}