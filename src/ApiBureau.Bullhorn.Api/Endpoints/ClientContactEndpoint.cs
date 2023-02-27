namespace ApiBureau.Bullhorn.Api.Endpoints;

public class ClientContactEndpoint : SearchBaseEndpoint<ClientContactDto>
{
    private const string EntityDefaultFields = "id,clientCorporation,isDeleted,firstName,lastName,email,dateAdded,dateLastModified,owner";

    public ClientContactEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    public Task<Result<ChangeResponse>> AddAsync(ClientContactDto dto)
        => ApiConnection.PutAsJsonAsync(EntityType.ClientContact, dto);

    //public async Task<ClientContactDto> FindClientContactIdByEmail2Async(string emailQuery)
    //{
    //    var query = $"search/ClientContact?fields=id,firstName,lastName&query={emailQuery} AND isDeleted:0";

    //    var response = await ApiConnection.ApiGetAsync(query);

    //    var searchResponse = JsonConvert.DeserializeObject<SearchResponse>(
    //        await response.Content.ReadAsStringAsync());

    //    var clientContact = searchResponse?.Data?.FirstOrDefault()?.ToObject<ClientContactDto>();

    //    return clientContact;
    //}

    //ToDo check ToObject
    //public async Task<ClientContactDto?> FindClientContactIdByEmailAsync(string email)
    //{
    //    var query = $"search/ClientContact?fields=id,firstName,lastName&query=email:({email}) AND isDeleted:0";

    //    var response = await ApiConnection.ApiGetAsync(query);

    //    //var searchResponse = JsonConvert.DeserializeObject<SearchResponse>(
    //    //    await response.Content.ReadAsStringAsync());

    //    var searchResponse = await response.DeserializeAsync<SearchResponse>();

    //    return searchResponse?.Data?.FirstOrDefault()?.ToObject<ClientContactDto>();
    //}

    public async Task<List<ClientContactDto>> FindClientContactIdByEmailAsync(List<string> emails)
    {
        var query = $"{RequestUrl}?fields=id,firstName,lastName,email&query=email:({ApiConnection.GetQuotedString(emails)}) AND isDeleted:0";

        return await ApiConnection.SearchAsync<ClientContactDto>(query);
    }
}