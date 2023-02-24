namespace ApiBureau.Bullhorn.Api.Endpoints;

public class JobSubmissionEndpoint : QueryBaseEndpoint<JobSubmissionDto>
{
    private const string EntityDefaultFields = "id,dateAdded,dateLastModified,status,isDeleted,candidate,jobOrder,sendingUser";

    public JobSubmissionEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Http POST /entity/JobSubmission/{jobSubmissionId}
    /// </summary>
    /// <returns></returns>
    public Task<HttpResponseMessage> UpdateAsync(int jobSubmissionId, object data)
        => ApiConnection.PostAsJsonAsync(EntityType.JobSubmission, jobSubmissionId, data);
}