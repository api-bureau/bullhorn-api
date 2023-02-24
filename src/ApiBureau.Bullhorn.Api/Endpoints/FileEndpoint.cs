namespace ApiBureau.Bullhorn.Api.Endpoints;

public class FileEndpoint : BaseEndpoint
{
    public FileEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl) { }

    public async Task<FileDto?> GetFileAsync(string entityType, int entityId, int fileId)
    {
        //ToDo refactor and use EntityType
        var query = $"{RequestUrl}/{entityType}/{entityId}/{fileId}?";

        var response = await ApiConnection.GetAsync(query);

        var fileResponse = await response.DeserializeAsync<FileResponse<FileDto>>();

        return fileResponse?.File;
    }
}