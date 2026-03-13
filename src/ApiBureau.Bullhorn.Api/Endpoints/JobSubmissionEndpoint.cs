namespace ApiBureau.Bullhorn.Api.Endpoints;

public class JobSubmissionEndpoint : QueryEndpointBase<JobSubmissionDto>
{
    private const string EntityDefaultFields = "id,dateAdded,dateLastModified,status,isDeleted,candidate,jobOrder,sendingUser";

    public JobSubmissionEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Adds a new job submission using an anonymous or partial object, sending only the properties defined on it.
    /// </summary>
    /// <remarks>
    /// Prefer this overload when only a subset of job submission fields need to be set, avoiding empty values
    /// overwriting existing Bullhorn data. The object is serialised as-is to the API.
    /// </remarks>
    /// <param name="dto">An object whose properties represent the job submission fields to add.</param>
    /// <param name="token">A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result{ChangeResponse}"/> indicating the outcome of the operation.</returns>
    public async Task<Result<ChangeResponse>> AddAsync(object dto, CancellationToken token)
        => await ApiConnection.PutAsJsonAsync(EntityType.JobSubmission, dto, token);

    /// <summary>
    /// Http POST /entity/JobSubmission/{jobSubmissionId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int jobSubmissionId, object data, CancellationToken token)
        => ApiConnection.PostAsJsonAsync(EntityType.JobSubmission, jobSubmissionId, data, token);
}