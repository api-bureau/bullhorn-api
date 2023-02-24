namespace ApiBureau.Bullhorn.Api.Endpoints;

public class ResumeEndpoint : BaseEndpoint
{
    public ResumeEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl) { }

    public async Task<ResumeDto?> ParseAsync(FileDto fileDto)
    {
        var query = $"{RequestUrl}/parseToCandidate?format=text&populateDescription=html&";

        var content = new MultipartFormDataContent
        {
            { new ByteArrayContent(Convert.FromBase64String(fileDto.FileContent)), "resume", string.IsNullOrWhiteSpace(fileDto.Name) ? "temp-file-name" : fileDto.Name }
        };

        //content.Add(new ByteArrayContent(Convert.FromBase64String(fileDto.FileContent)), "resume", string.IsNullOrWhiteSpace(fileDto.Name) ? "temp-file-name" : fileDto.Name);

        var response = await ApiConnection.PostAsync(query, content);

        return await response.DeserializeAsync<ResumeDto>();
    }
}