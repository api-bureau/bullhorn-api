namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class FileEndpoint
{
    private readonly BullhornHttpClient _client;

    internal FileEndpoint(BullhornHttpClient client, string requestUrl)
    {
        _client = client;
        RequestUrl = requestUrl;
    }

    private string RequestUrl { get; }

    public async Task<FileDto?> GetFileAsync(EntityType entityType, int entityId, int fileId, CancellationToken cancellationToken)
    {
        var query = $"{RequestUrl}/{entityType}/{entityId}/{fileId}?";

        var response = await _client.GetAsync(query, cancellationToken);

        var fileResponse = await response.DeserializeAsync<FileResponse<FileDto>>();

        return fileResponse?.File;
    }
}