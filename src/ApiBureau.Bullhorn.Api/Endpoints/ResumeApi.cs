using ApiBureau.Bullhorn.Api.Dtos;
using ApiBureau.Bullhorn.Api.Helpers;

namespace ApiBureau.Bullhorn.Api.Endpoints
{
    public class ResumeApi
    {
        private readonly BullhornApi _bullhornApi;

        public ResumeApi(BullhornApi bullhornApi) => _bullhornApi = bullhornApi;

        public async Task<ResumeDto> ParseAsync(FileDto fileDto)
        {
            var query = "resume/parseToCandidate?format=text&populateDescription=html&";

            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(Convert.FromBase64String(fileDto.FileContent)), "resume", string.IsNullOrWhiteSpace(fileDto.Name) ? "temp-file-name" : fileDto.Name);

            var response = await _bullhornApi.ApiPostAsync(query, content);

            return await response.DeserializeAsync<ResumeDto>();
        }
    }
}
