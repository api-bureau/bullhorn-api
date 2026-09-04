namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class ResumeEndpoint
{
    private readonly BullhornHttpClient _client;

    internal ResumeEndpoint(BullhornHttpClient client, string requestUrl)
    {
        _client = client;
        RequestUrl = requestUrl;
    }

    private string RequestUrl { get; }

    public async Task<ResumeDto?> ParseAsync(FileDto fileDto, CancellationToken cancellationToken)
    {
        var query = $"{RequestUrl}/parseToCandidate?format=text&populateDescription=html&";

        var content = new MultipartFormDataContent
        {
            { new ByteArrayContent(Convert.FromBase64String(fileDto.FileContent)), "resume", string.IsNullOrWhiteSpace(fileDto.Name) ? "temp-file-name" : fileDto.Name }
        };

        //content.Add(new ByteArrayContent(Convert.FromBase64String(fileDto.FileContent)), "resume", string.IsNullOrWhiteSpace(fileDto.Name) ? "temp-file-name" : fileDto.Name);

        var response = await _client.PostAsync(query, content, cancellationToken);

        return await response.DeserializeAsync<ResumeDto>();
    }
}