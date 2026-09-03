namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class FileEndpoint
{
    private readonly BullhornHttpClient _client;

    internal FileEndpoint(BullhornHttpClient client, string requestUrl)
    {
        _client = client;
        RequestUrl = requestUrl;
    }

    public string RequestUrl { get; }

    public async Task<FileDto?> GetFileAsync(EntityType entityType, int entityId, int fileId, CancellationToken token)
    {
        var query = $"{RequestUrl}/{entityType}/{entityId}/{fileId}?";

        var response = await _client.GetAsync(query, token);

        var fileResponse = await response.DeserializeAsync<FileResponse<FileDto>>();

        return fileResponse?.File;
    }

    [Obsolete]
    public async Task<FileDto?> GetFileAsync(string entityType, int entityId, int fileId, CancellationToken token)
    {
        //ToDo refactor and use EntityType
        var query = $"{RequestUrl}/{entityType}/{entityId}/{fileId}?";

        var response = await _client.GetAsync(query, token);

        var fileResponse = await response.DeserializeAsync<FileResponse<FileDto>>();

        return fileResponse?.File;
    }
}