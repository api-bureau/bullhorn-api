using ApiBureau.Bullhorn.Api.Extensions;

namespace ApiBureau.Bullhorn.Api.Endpoints;

public class CandidateWorkHistoryApi : BaseEndpoint
{
    public static readonly string DefaultFields = "id,dateAdded,isDeleted,candidate(id)";

    public CandidateWorkHistoryApi(ApiConnection apiConnection) : base(apiConnection) { }

    public async Task<List<CandidateWorkHistoryDto>> GetDeletedAsync(DateTime dateAddedFrom, string? fields = null)
    {
        var query = $"CandidateWorkHistory?fields={fields ?? DefaultFields}&where=isDeleted=true AND dateAdded>{dateAddedFrom.Timestamp()}";

        return await ApiConnection.QueryAsync<CandidateWorkHistoryDto>(query);
    }
}