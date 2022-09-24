using ApiBureau.Bullhorn.Api.Dtos;
using ApiBureau.Bullhorn.Api.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiBureau.Bullhorn.Api.Endpoints
{
    public class CandidateWorkHistoryApi
    {
        private readonly BullhornClient _bullhornApi;
        public static readonly string DefaultFields = "id,dateAdded,isDeleted,candidate(id)";

        public CandidateWorkHistoryApi(BullhornClient bullhornApi) => _bullhornApi = bullhornApi;

        public async Task<List<CandidateWorkHistoryDto>> GetDeletedAsync(DateTime dateAddedFrom, string? fields = null)
        {
            var query = $"CandidateWorkHistory?fields={fields ?? DefaultFields}&where=isDeleted=true AND dateAdded>{dateAddedFrom.Timestamp()}";

            return await _bullhornApi.QueryAsync<CandidateWorkHistoryDto>(query);
        }
    }
}
