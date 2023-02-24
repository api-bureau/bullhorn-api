namespace ApiBureau.Bullhorn.Api.Endpoints;

public class CandidateWorkHistoryEndpoint : QueryBaseEndpoint<CandidateWorkHistoryDto>
{
    private const string EntityDefaultFields = "id,dateAdded,isDeleted,candidate(id)";

    public CandidateWorkHistoryEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    public async Task<List<CandidateWorkHistoryDto>> GetDeletedFromAsync(DateTime dateAddedFrom, string? fields = null)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&where=isDeleted=true AND dateAdded>={dateAddedFrom.Timestamp()}";

        return await ApiConnection.QueryAsync<CandidateWorkHistoryDto>(query);
    }
}