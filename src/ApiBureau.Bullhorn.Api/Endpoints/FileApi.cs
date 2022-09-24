using ApiBureau.Bullhorn.Api.Helpers;

namespace ApiBureau.Bullhorn.Api.Endpoints;

public class FileApi : BaseEndpoint
{
    public FileApi(ApiConnection apiConnection) : base(apiConnection) { }

    public async Task<FileDto?> GetFileAsync(string entityType, int entityId, int fileId)
    {
        var query = $"file/{entityType}/{entityId}/{fileId}?";

        var response = await ApiConnection.GetAsync(query);

        var fileResponse = await response.DeserializeAsync<FileResponse>();

        return fileResponse?.File;
    }
}
