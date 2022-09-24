using ApiBureau.Bullhorn.Api.Dtos;

namespace ApiBureau.Bullhorn.Api.Endpoints
{
    public class SendoutApi
    {
        private readonly BullhornClient _bullhornApi;

        public SendoutApi(BullhornClient bullhornApi) => _bullhornApi = bullhornApi;

        /// <summary>
        /// Get all from a specific timestamp
        /// </summary>
        /// <param name="timestampFrom"></param>
        /// <returns></returns>
        public async Task<List<SendoutDto>> GetAsync(long timestampFrom)
        {
            var query = $"Sendout?fields=id,candidate,user,dateAdded,jobOrder,clientContact,clientCorporation&where=dateAdded>={timestampFrom}";

            return await _bullhornApi.QueryAsync<SendoutDto>(query);
        }
    }
}
