namespace ApiBureau.Bullhorn.Api.Endpoints;

public class JobOrderEndpoint : BaseEndpoint
{
    public static readonly string DefaultFields = "id,dateAdded,dateLastModified,status,title,source,owner,isOpen,isDeleted,clientContact,clientCorporation";

    public JobOrderEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    public async Task<JobOrderDto> GetAsync(int id, string? fields = null)
    {
        var query = $"JobOrder/{id}?fields={fields ?? DefaultFields}";

        return await ApiConnection.EntityAsync<JobOrderDto>(query);
    }

    public async Task<List<JobOrderDto>> GetAsync(List<int> ids, string? fields = null)
    {
        if (ids.Count == 1)
            return new List<JobOrderDto> { await GetAsync(ids[0], fields ?? DefaultFields) };

        var query = $"JobOrder/{string.Join(",", ids)}?fields={fields ?? DefaultFields}";

        return await ApiConnection.EntityAsync<List<JobOrderDto>>(query);
    }

    public async Task<List<JobOrderDto>> GetNewAndUpdatedFromAsync(long timestampFrom)
    {
        var query = $"JobOrder?fields={DefaultFields}&where=dateAdded>{timestampFrom} OR dateLastModified>{timestampFrom}";

        return await ApiConnection.QueryAsync<JobOrderDto>(query);
    }

    /// <summary>
    /// Http POST /entity/JobOrder/{jobOrderId}
    /// </summary>
    /// <returns></returns>
    public Task<HttpResponseMessage> UpdateAsync(int jobOrderId, object data)
        => ApiConnection.PostAsJsonAsync(EntityType.JobOrder, jobOrderId, data);
}
