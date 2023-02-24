using ApiBureau.Bullhorn.Api.Helpers;
using System.Text;
using System.Text.Json;

namespace ApiBureau.Bullhorn.Api.Endpoints;

public class CandidateEndpoint : SearchBaseEndpoint<CandidateDto>
{
    private const string EntityDefaultFields = "id,status,isDeleted,firstName,lastName,email,dateAdded,dateLastModified,source,owner";

    public CandidateEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    public async Task AddOldAsync(CandidateDto dto)
        => await ApiConnection.ApiPutAsync($"entity/{RequestUrl}", new StringContent(JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), Encoding.UTF8, "application/json"));

    public async Task<HttpResponseMessage> AddAsync(CandidateDto dto)
        => await ApiConnection.PutAsJsonAsync(EntityType.Candidate, dto);

    public async Task<HttpResponseMessage> AddAsync(object dto)
        => await ApiConnection.PutAsJsonAsync(EntityType.Candidate, dto);

    public async Task<int> GetFilesCount(int id)
    {
        var query = $"entity/{RequestUrl}/{id}/fileAttachments?fields=id";

        var response = await ApiConnection.GetAsync(query);

        //var entityResponse = JsonConvert.DeserializeObject<QueryResponse>(await response.Content.ReadAsStringAsync(),
        //    new JsonSerializerSettings
        //    {
        //        MissingMemberHandling = MissingMemberHandling.Ignore,
        //        NullValueHandling = NullValueHandling.Ignore
        //    });

        //return (await BullhornApi.DeserializeAsync<QueryResponse>(response)).Total;

        //if (response == null) return 0;

        return (await response.DeserializeAsync<QueryResponse>())?.Total ?? 0;
    }

    //public async Task<List<CandidateDto>> GetNewFromAsync(DateTime dateTime)
    //{
    //    var query = $"{RequestUrl}?fields=id,status,isDeleted,firstName,lastName,email,dateAdded,source,owner&query=dateAdded:[{dateTime:yyyyMMddHHmmss} TO *]";

    //    return await ApiConnection.SearchAsync<CandidateDto>(query);
    //}

    //public async Task<List<CandidateDto>> GetNewAndUpdatedFromAsync(DateTime dateTime)
    //{
    //    var query = $"{RequestUrl}?fields={DefaultFields}&query=dateAdded:[{dateTime:yyyyMMddHHmmss} TO *] OR dateLastModified:[{dateTime:yyyyMMddHHmmss} TO *]";

    //    return await ApiConnection.SearchAsync<CandidateDto>(query);
    //}

    //public async Task<List<CandidateDto>> GetUpdatedCandidatesFromAsync(DateTime dateTime)
    //{
    //    var query = $"{RequestUrl}?fields=id&query=dateLastModified:[{dateTime:yyyyMMdd} TO *]";

    //    return await ApiConnection.SearchAsync<CandidateDto>(query);
    //}

    public async Task<CandidateDto?> FindCandidateIdByEmailAsync(string email)
    {
        var query = $"search/{RequestUrl}?fields=id,firstName,lastName&query=email:\"{email}\" AND isDeleted:0";

        var response = await ApiConnection.GetAsync(query);

        //var searchResponse = JsonConvert.DeserializeObject<SearchResponse>(
        //    await response.Content.ReadAsStringAsync());

        var searchResponse = await response.DeserializeAsync<SearchResponse2<CandidateDto>>();

        //https://stackoverflow.com/questions/58138793/system-text-json-jsonelement-toobject-workaround
        return searchResponse?.Data?.FirstOrDefault();
    }

    public async Task<List<CandidateDto>> FindCandidateIdByEmailAsync(List<string> emails)
    {
        var query = $"{RequestUrl}?fields=id,firstName,lastName,email&query=email:({ApiConnection.GetQuotedString(emails)}) AND isDeleted:0";

        return await ApiConnection.SearchAsync<CandidateDto>(query);

        //var result = await ApiConnection.ApiSearchAsync(query, BullhornApi.QueryCount);

        //return ApiConnection.MapResults<CandidateDto>(result.Data);
    }
}