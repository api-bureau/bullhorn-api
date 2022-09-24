using ApiBureau.Bullhorn.Api.Dtos;

namespace ApiBureau.Bullhorn.Api.Endpoints
{
    public class PlacementCommissionApi
    {
        private readonly BullhornClient _bullhornApi;

        public PlacementCommissionApi(BullhornClient bullhornApi) => _bullhornApi = bullhornApi;

        public async Task<List<PlacementCommissionDto>> GetFromAsync(long timestampFrom)
        {
            var query = $"PlacementCommission?fields=id,commissionPercentage,dateAdded,dateLastModified,placement(id),user(id),status,role&where=dateAdded>{timestampFrom}";

            return await _bullhornApi.QueryAsync<PlacementCommissionDto>(query);
        }

        public async Task<List<PlacementCommissionDto>> GetNewAndUpdatedFromAsync(long timestampFrom)
        {
            var query = $"PlacementCommission?fields=id,commissionPercentage,dateAdded,dateLastModified,placement(id),user(id),status,role&where=dateAdded>{timestampFrom} OR dateLastModified>{timestampFrom}";

            return await _bullhornApi.QueryAsync<PlacementCommissionDto>(query);
        }
    }
}
