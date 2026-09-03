using ApiBureau.Bullhorn.Api.Internals;

namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class EntityEditHistoryEndpoint
{
    private const string EntityDefaultFields = "id,display,columnName,newValue,oldValue,editHistory(dateAdded,modifyingPerson,targetEntity)";
    private readonly QueryOperations<EditHistoryFieldChangeDto> _operations;

    internal EntityEditHistoryEndpoint(BullhornHttpClient client)
        => _operations = new(client, string.Empty, EntityDefaultFields);

    public string RequestUrl => string.Empty;

    public async Task<List<EditHistoryFieldChangeDto>> GetFromAsync(EntityType entityType, long timestampFrom, CancellationToken token)
    {
        var query = $"{entityType}EditHistoryFieldChange?fields={EntityDefaultFields}&where=editHistory.dateAdded>={timestampFrom}";

        return await _operations.ExecuteAsync(query, token);
    }

    public async Task<List<EditHistoryFieldChangeDto>> GetFromByColumnNameAsync(EntityType entityType, long timestampFrom, string columnName, CancellationToken token)
    {
        var query = $"{entityType}EditHistoryFieldChange?fields={EntityDefaultFields}&where=editHistory.dateAdded>={timestampFrom} AND columnName='{columnName}'";

        return await _operations.ExecuteAsync(query, token);
    }
}