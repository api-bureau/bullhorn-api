namespace ApiBureau.Bullhorn.Api.Endpoints;

public class JobSubmissionHistoryApi : BaseEndpoint
{
    public static readonly string DefaultFields = "id,dateAdded,status,modifyingUser,jobSubmission";

    public JobSubmissionHistoryApi(ApiConnection apiConnection) : base(apiConnection) { }

    public Task<List<JobSubmissionHistoryDto>> GetByCandidateIdAsync(int id)
    {
        var query = $"JobSubmissionHistory?fields=id,dateAdded,status,jobSubmission(id,status,jobOrder(id))&where=jobSubmission.candidate.id={id}";

        return ApiConnection.QueryAsync<JobSubmissionHistoryDto>(query);
    }

    public Task<List<JobSubmissionHistoryDto>> GetNewAndUpdatedFromAsync(long timestampFrom)
    {
        var query = $"JobSubmissionHistory?fields={DefaultFields}&where=dateAdded>{timestampFrom}";

        return ApiConnection.QueryAsync<JobSubmissionHistoryDto>(query);
    }
}
