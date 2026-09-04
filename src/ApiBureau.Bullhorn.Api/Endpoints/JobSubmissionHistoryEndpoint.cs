namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class JobSubmissionHistoryEndpoint : QueryEndpointBase<JobSubmissionHistoryDto>
{
    private const string EntityDefaultFields = "id,dateAdded,status,modifyingUser,jobSubmission";

    internal JobSubmissionHistoryEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    public Task<List<JobSubmissionHistoryDto>> GetByCandidateIdAsync(int candidateId, string fields = "id,dateAdded,status,jobSubmission(id,status,jobOrder(id))", CancellationToken cancellationToken = default)
    {
        var query = $"{RequestUrl}?fields={fields}&where=jobSubmission.candidate.id={candidateId}";

        return ExecuteQueryAsync(query, cancellationToken);
    }
}