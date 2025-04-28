namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// Note entity with default fields: id,action,commentingPerson,dateAdded,dateLastModified,isDeleted,comments,minutesSpent,personReference
/// </summary>
public class NoteEndpoint : SearchBaseEndpoint<NoteDto>
{
    private const string AllFields = "id,action,commentingPerson,dateAdded,dateLastModified,isDeleted,comments,minutesSpent,personReference";

    public NoteEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, AllFields) { }

    //public async Task AddOldAsync(NoteDto noteDto)
    //    => await ApiConnection.ApiPutAsync("entity/Note", new StringContent(JsonSerializer.Serialize(noteDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), Encoding.UTF8, "application/json"));

    public async Task<Result<ChangeResponse>> AddAsync(NoteDto dto)
        => await ApiConnection.PutAsJsonAsync(EntityType.Note, dto);

    [Obsolete("Use SearchFromAsync", true)]
    public async Task<List<NoteDto>> GetFromAsync(DateTime dateTime, CancellationToken token)
    {
        var query = $"{RequestUrl}?fields={AllFields}&query=dateAdded:[{dateTime:yyyyMMddHHmmss} TO *]";

        return await ApiConnection.SearchAsync<NoteDto>(query, token: token);
    }

    [Obsolete("Use SearchNewAndUpdatedFromAsync", true)]
    public async Task<List<NoteDto>> GetNewAndUpdatedFromAsync(DateTime dateTime, CancellationToken token)
    {
        var query = $"{RequestUrl}?fields={AllFields}&query=dateAdded:[{dateTime:yyyyMMddHHmmss} TO *] OR dateLastModified:[{dateTime:yyyyMMddHHmmss} TO *]";

        return await ApiConnection.SearchAsync<NoteDto>(query, token: token) ?? new List<NoteDto>();
    }

    [Obsolete("Use SearchFromToAsync", true)]
    public async Task<List<NoteDto>> GetFromToAsync(DateTime dateTimeFrom, DateTime dateTimeTo, CancellationToken token)
    {
        var query = $"{RequestUrl}?fields={AllFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO {dateTimeTo:yyyyMMddHHmmss}]";

        return await ApiConnection.SearchAsync<NoteDto>(query, token: token);
    }

    public async Task<List<NoteDto>> GetNotesAsync(string userQuery, string fields = AllFields, CancellationToken token = default)
    {
        var query = $"{RequestUrl}?fields={fields}&query={userQuery}&sort=noteID"; // case must be noteID

        return await ApiConnection.SearchAsync<NoteDto>(query, token: token);
    }
}