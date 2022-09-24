namespace ApiBureau.Bullhorn.Api.Endpoints
{
    public class ClientContactEndpoint : BaseEndpoint
    {
        public readonly string DefaultFields = "id,clientCorporation,isDeleted,firstName,lastName,email,dateAdded,dateLastModified,owner";

        public ClientContactEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

        public async Task<ClientContactDto> GetAsync(int id, string? fields = null)
        {
            var query = $"ClientContact/{id}?fields={fields ?? DefaultFields}";

            return await ApiConnection.EntityAsync<ClientContactDto>(query);
        }

        public async Task<List<ClientContactDto>> GetAsync(List<int> ids, string? fields = null)
        {
            if (ids.Count == 1)
                return new List<ClientContactDto> { await GetAsync(ids[0], fields ?? DefaultFields) };

            var query = $"ClientContact/{string.Join(",", ids)}?fields={fields ?? DefaultFields}";

            return await ApiConnection.EntityAsync<List<ClientContactDto>>(query);
        }

        public async Task<List<ClientContactDto>> GetAsync(DateTime from, DateTime to)
        {
            var query = $"ClientContact?fields={DefaultFields}&query=dateAdded:[{from:yyyyMMdd} TO {to:yyyyMMdd}]";

            return await ApiConnection.SearchAsync<ClientContactDto>(query);
        }

        public Task<HttpResponseMessage> AddAsync(ClientContactDto dto)
            => ApiConnection.PutAsJsonAsync(EntityType.ClientContact, dto);

        public async Task<List<ClientContactDto>> GetNewClientContactsAsync(DateTime dateTime)
        {
            var query = $"ClientContact?fields=id,firstName,lastName,email&query=dateAdded:[{dateTime:yyyyMMdd} TO *]";

            return await ApiConnection.SearchAsync<ClientContactDto>(query);
        }

        public async Task<List<ClientContactDto>> GetNewAndUpdatedFromAsync(DateTime dateTime)
        {
            var query = $"ClientContact?fields={DefaultFields}&query=dateAdded:[{dateTime:yyyyMMdd} TO *] OR dateLastModified:[{dateTime:yyyyMMddHHmmss} TO *]";

            return await ApiConnection.SearchAsync<ClientContactDto>(query);
        }

        public async Task<List<ClientContactDto>> GetUpdatedClientContactsAsync(DateTime dateTime)
        {
            var query = $"ClientContact?fields=id&query=dateLastModified:[{dateTime:yyyyMMdd} TO *]";

            return await ApiConnection.SearchAsync<ClientContactDto>(query);
        }

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
            var query = $"ClientContact?fields=id,firstName,lastName,email&query=email:({ApiConnection.GetQuotedString(emails)}) AND isDeleted:0";

            return await ApiConnection.SearchAsync<ClientContactDto>(query);
        }
    }
}
