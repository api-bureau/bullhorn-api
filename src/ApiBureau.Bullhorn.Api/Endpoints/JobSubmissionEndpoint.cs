namespace ApiBureau.Bullhorn.Api.Endpoints;

public class JobSubmissionEndpoint : BaseEndpoint
{
    public static readonly string DefaultFields = "id,dateAdded,dateLastModified,status,isDeleted,candidate,jobOrder,sendingUser";

    public JobSubmissionEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    public Task<JobSubmissionDto> GetAsync(int id, string? fields = null)
        => ApiConnection.EntityAsync<JobSubmissionDto>($"JobSubmission/{id}?fields={fields ?? DefaultFields}");

    public Task<List<JobSubmissionDto>> GetNewAndUpdatedFromAsync(long timestampFrom)
    {
        var query = $"JobSubmission?fields={DefaultFields}&where=dateAdded>{timestampFrom} OR dateLastModified>{timestampFrom}";

        return ApiConnection.QueryAsync<JobSubmissionDto>(query);
    }

    public async Task<List<JobSubmissionDto>> GetAsync(List<int> ids, string? fields = null)
    {
        if (ids.Count == 1)
            return new List<JobSubmissionDto> { await GetAsync(ids[0], fields ?? DefaultFields) };

        var query = $"JobSubmission/{string.Join(",", ids)}?fields={fields ?? DefaultFields}";

        return await ApiConnection.EntityAsync<List<JobSubmissionDto>>(query);
    }

    /// <summary>
    /// Http POST /entity/JobSubmission/{jobSubmissionId}
    /// </summary>
    /// <returns></returns>
    public Task<HttpResponseMessage> UpdateAsync(int jobSubmissionId, object data)
        => ApiConnection.PostAsJsonAsync(EntityType.JobSubmission, jobSubmissionId, data);
}