namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// JobOrder entity with default fields: id,dateAdded,dateLastModified,status,title,source,owner,isOpen,isDeleted,clientContact,clientCorporation
/// </summary>
public class JobOrderEndpoint : QueryEndpointBase<JobOrderDto>
{
    private const string EntityDefaultFields = "id,dateAdded,dateLastModified,status,title,source,owner,isOpen,isDeleted,clientContact,clientCorporation";

    public JobOrderEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    [Obsolete("Use QueryNewAndUpdatedFromAsync", true)]
    public async Task<List<JobOrderDto>> GetNewAndUpdatedFromAsync(long timestampFrom, CancellationToken token)
    {
        var query = $"{RequestUrl}?fields={DefaultFields}&where=dateAdded>{timestampFrom} OR dateLastModified>{timestampFrom}";

        return await ApiConnection.QueryAsync<JobOrderDto>(query, token);
    }

    /// <summary>
    /// Adds a new job order using an anonymous or partial object, sending only the properties defined on it.
    /// </summary>
    /// <remarks>
    /// Prefer this overload when only a subset of job order fields need to be set, avoiding empty values
    /// overwriting existing Bullhorn data. The object is serialised as-is to the API.
    /// </remarks>
    /// <param name="dto">An object whose properties represent the job order fields to add.</param>
    /// <param name="token">A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result{ChangeResponse}"/> indicating the outcome of the operation.</returns>
    public async Task<Result<ChangeResponse>> AddAsync(object dto, CancellationToken token)
        => await ApiConnection.PutAsJsonAsync(EntityType.JobOrder, dto, token);

    /// <summary>
    /// Http POST /entity/JobOrder/{jobOrderId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int jobOrderId, object data, CancellationToken token = default)
        => ApiConnection.PostAsJsonAsync(EntityType.JobOrder, jobOrderId, data, token);
}