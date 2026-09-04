namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class CandidateWorkHistoryEndpoint : QueryEndpointBase<CandidateWorkHistoryDto>
{
    private const string EntityDefaultFields = "id,dateAdded,isDeleted,candidate(id)";

    internal CandidateWorkHistoryEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    public async Task<List<CandidateWorkHistoryDto>> GetDeletedFromAsync(DateTime dateAddedFrom, string? fields = null, CancellationToken cancellationToken = default)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&where=isDeleted=true AND dateAdded>={dateAddedFrom.Timestamp()}";

        return await ExecuteQueryAsync(query, cancellationToken);
    }
}