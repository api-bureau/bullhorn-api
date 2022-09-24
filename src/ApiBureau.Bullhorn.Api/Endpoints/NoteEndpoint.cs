namespace ApiBureau.Bullhorn.Api.Endpoints;

public class NoteEndpoint : BaseEndpoint
{
    private const string AllFields = "id,action,commentingPerson,dateAdded,dateLastModified,isDeleted,comments,minutesSpent,personReference";

    public NoteEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    //public async Task AddOldAsync(NoteDto noteDto)
    //    => await ApiConnection.ApiPutAsync("entity/Note", new StringContent(JsonSerializer.Serialize(noteDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), Encoding.UTF8, "application/json"));

    public async Task<HttpResponseMessage> AddAsync(NoteDto dto)
        => await ApiConnection.PutAsJsonAsync(EntityType.Note, dto);

    public async Task<List<NoteDto>> GetFromAsync(DateTime dateTime)
    {
        var query = $"Note?fields={AllFields}&query=dateAdded:[{dateTime:yyyyMMddHHmmss} TO *]";

        return await ApiConnection.SearchAsync<NoteDto>(query);
    }

    public async Task<List<NoteDto>> GetNewAndUpdatedFromAsync(DateTime dateTime)
    {
        var query = $"Note?fields={AllFields}&query=dateAdded:[{dateTime:yyyyMMddHHmmss} TO *] OR dateLastModified:[{dateTime:yyyyMMddHHmmss} TO *]";

        return await ApiConnection.SearchAsync<NoteDto>(query) ?? new List<NoteDto>();
    }

    public async Task<List<NoteDto>> GetFromToAsync(DateTime dateTimeFrom, DateTime dateTimeTo)
    {
        var query = $"Note?fields={AllFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO {dateTimeTo:yyyyMMddHHmmss}]";

        return await ApiConnection.SearchAsync<NoteDto>(query);
    }

    public async Task<List<NoteDto>> GetNotesAsync(string userQuery, string fields = AllFields)
    {
        var query = $"Note?fields={fields}&query={userQuery}&sort=noteID"; // case must be noteID

        return await ApiConnection.SearchAsync<NoteDto>(query);
    }
}
