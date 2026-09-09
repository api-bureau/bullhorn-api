namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// Note entity with default fields: id,action,commentingPerson,dateAdded,dateLastModified,isDeleted,comments,minutesSpent,personReference
/// </summary>
public sealed class NoteEndpoint : SearchEndpointBase<NoteDto>
{
    private const string AllFields = "id,action,commentingPerson,dateAdded,dateLastModified,isDeleted,comments,minutesSpent,personReference";

    internal NoteEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, AllFields) { }

    public async Task<Result<ChangeResponse, ErrorResponse>> AddAsync(NoteDto dto, CancellationToken cancellationToken)
        => await HttpClient.PutAsJsonAsync(EntityType.Note, dto, cancellationToken);

    public async Task<List<NoteDto>> GetNotesAsync(string userQuery, string fields = AllFields, CancellationToken cancellationToken = default)
    {
        var query = $"{RequestUrl}?fields={fields}&query={userQuery}&sort=noteID"; // case must be noteID

        return await ExecuteSearchAsync(query, cancellationToken);
    }
}