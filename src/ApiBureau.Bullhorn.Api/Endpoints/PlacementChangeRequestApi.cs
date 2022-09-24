using ApiBureau.Bullhorn.Api.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiBureau.Bullhorn.Api.Endpoints
{
    public class PlacementChangeRequestApi
    {
        private readonly BullhornClient _bullhornApi;

        public PlacementChangeRequestApi(BullhornClient bullhornApi) => _bullhornApi = bullhornApi;

        public async Task<List<PlacementChangeRequestDto>> GetNewAndUpdatedFromAsync(long timestampFrom)
        {
            var query = $"PlacementChangeRequest?fields=id,dateAdded,dateLastModified,requestStatus,requestType,placement(id),customText12&where=dateAdded>{timestampFrom} OR dateLastModified>{timestampFrom}";

            return await _bullhornApi.QueryAsync<PlacementChangeRequestDto>(query);
        }
    }
}
