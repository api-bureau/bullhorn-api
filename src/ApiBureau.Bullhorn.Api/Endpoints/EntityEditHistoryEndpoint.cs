namespace ApiBureau.Bullhorn.Api.Endpoints;

public class EntityEditHistoryEndpoint : EndpointBase
{
    public EntityEditHistoryEndpoint(ApiConnection apiConnection) : base(apiConnection, "") { }

    private const string EntityDefaultFields = "id,display,columnName,newValue,oldValue,editHistory(dateAdded,modifyingPerson,targetEntity)";

    public async Task<List<EditHistoryFieldChangeDto>> GetFromAsync(EntityType entityType, long timestampFrom, CancellationToken token)
    {
        var query = $"{entityType}EditHistoryFieldChange?fields={EntityDefaultFields}&where=editHistory.dateAdded>={timestampFrom}";

        return await ApiConnection.QueryAsync<EditHistoryFieldChangeDto>(query, token);
    }

    public async Task<List<EditHistoryFieldChangeDto>> GetFromByColumnNameAsync(EntityType entityType, long timestampFrom, string columnName, CancellationToken token)
    {
        var query = $"{entityType}EditHistoryFieldChange?fields={EntityDefaultFields}&where=editHistory.dateAdded>={timestampFrom} AND columnName='{columnName}'";

        return await ApiConnection.QueryAsync<EditHistoryFieldChangeDto>(query, token);
    }
}