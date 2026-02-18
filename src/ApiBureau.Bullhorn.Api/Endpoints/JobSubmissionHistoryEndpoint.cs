namespace ApiBureau.Bullhorn.Api.Endpoints;

public class JobSubmissionHistoryEndpoint : QueryEndpointBase<JobSubmissionHistoryDto>
{
    private const string EntityDefaultFields = "id,dateAdded,status,modifyingUser,jobSubmission";

    public JobSubmissionHistoryEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    public Task<List<JobSubmissionHistoryDto>> GetByCandidateIdAsync(int candidateId, string fields = "id,dateAdded,status,jobSubmission(id,status,jobOrder(id))", CancellationToken token = default)
    {
        var query = $"{RequestUrl}?fields={fields}&where=jobSubmission.candidate.id={candidateId}";

        return ApiConnection.QueryAsync<JobSubmissionHistoryDto>(query, token);
    }
}