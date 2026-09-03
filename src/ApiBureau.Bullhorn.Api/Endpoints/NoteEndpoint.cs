namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// Note entity with default fields: id,action,commentingPerson,dateAdded,dateLastModified,isDeleted,comments,minutesSpent,personReference
/// </summary>
public class NoteEndpoint : SearchEndpointBase<NoteDto>
{
    private const string AllFields = "id,action,commentingPerson,dateAdded,dateLastModified,isDeleted,comments,minutesSpent,personReference";

    public NoteEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, AllFields) { }

    public async Task<Result<ChangeResponse>> AddAsync(NoteDto dto, CancellationToken token)
        => await ApiConnection.PutAsJsonAsync(EntityType.Note, dto, token);

    public async Task<List<NoteDto>> GetNotesAsync(string userQuery, string fields = AllFields, CancellationToken token = default)
    {
        var query = $"{RequestUrl}?fields={fields}&query={userQuery}&sort=noteID"; // case must be noteID

        return await ApiConnection.SearchAsync<NoteDto>(query, token: token);
    }
}