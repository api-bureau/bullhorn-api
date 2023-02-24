using ApiBureau.Bullhorn.Api.Helpers;

namespace ApiBureau.Bullhorn.Api.Endpoints;

public class ResumeApi : BaseEndpoint
{
    public ResumeApi(ApiConnection apiConnection) : base(apiConnection) { }

    public async Task<ResumeDto?> ParseAsync(FileDto fileDto)
    {
        var query = "resume/parseToCandidate?format=text&populateDescription=html&";

        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Convert.FromBase64String(fileDto.FileContent)), "resume", string.IsNullOrWhiteSpace(fileDto.Name) ? "temp-file-name" : fileDto.Name);

        var response = await ApiConnection.PostAsync(query, content);

        return await response.DeserializeAsync<ResumeDto>();
    }
}