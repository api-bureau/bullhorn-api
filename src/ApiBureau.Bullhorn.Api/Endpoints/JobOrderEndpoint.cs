namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// JobOrder entity with default fields: id,dateAdded,dateLastModified,status,title,source,owner,isOpen,isDeleted,clientContact,clientCorporation
/// </summary>
public sealed class JobOrderEndpoint : QueryEndpointBase<JobOrderDto>
{
    private const string EntityDefaultFields = "id,dateAdded,dateLastModified,status,title,source,owner,isOpen,isDeleted,clientContact,clientCorporation";

    internal JobOrderEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Adds a new job order using an anonymous or partial object, sending only the properties defined on it.
    /// </summary>
    /// <remarks>
    /// Prefer this overload when only a subset of job order fields need to be set, avoiding empty values
    /// overwriting existing Bullhorn data. The object is serialised as-is to the API.
    /// </remarks>
    /// <param name="dto">An object whose properties represent the job order fields to add.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result{ChangeResponse}"/> indicating the outcome of the operation.</returns>
    public async Task<Result<ChangeResponse, ErrorResponse>> AddAsync(object dto, CancellationToken cancellationToken)
        => await HttpClient.PutAsJsonAsync(EntityType.JobOrder, dto, cancellationToken);

    /// <summary>
    /// Http POST /entity/JobOrder/{jobOrderId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse, ErrorResponse>> UpdateAsync(int jobOrderId, object data, CancellationToken cancellationToken = default)
        => HttpClient.PostAsJsonAsync(EntityType.JobOrder, jobOrderId, data, cancellationToken);
}
