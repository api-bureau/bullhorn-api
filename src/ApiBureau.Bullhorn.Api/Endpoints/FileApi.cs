using ApiBureau.Bullhorn.Api.Dtos;
using ApiBureau.Bullhorn.Api.Helpers;
using System.Threading.Tasks;

namespace ApiBureau.Bullhorn.Api.Endpoints
{
    public class FileApi
    {
        private readonly BullhornClient _bullhornApi;

        public FileApi(BullhornClient bullhornApi) => _bullhornApi = bullhornApi;

        public async Task<FileDto> GetFileAsync(string entityType, int entityId, int fileId)
        {
            var query = $"file/{entityType}/{entityId}/{fileId}?";

            var response = await _bullhornApi.ApiGetAsync(query);

            var fileResponse = await response.DeserializeAsync<FileResponse>();

            return fileResponse.File;
        }
    }
}
