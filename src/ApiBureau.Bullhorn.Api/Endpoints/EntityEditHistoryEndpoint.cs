namespace ApiBureau.Bullhorn.Api.Endpoints;

public class EntityEditHistoryEndpoint : BaseEndpoint
{
    public EntityEditHistoryEndpoint(ApiConnection apiConnection) : base(apiConnection, "") { }

    private const string EntityDefaultFields = "id,display,columnName,newValue,oldValue,editHistory(dateAdded,modifyingPerson,targetEntity)";

    public async Task<List<EditHistoryFieldChangeDto>> GetFromAsync(EntityType entityType, long timestampFrom)
    {
        var query = $"{entityType}EditHistoryFieldChange?fields={EntityDefaultFields}&where=editHistory.dateAdded>={timestampFrom}";

        return await ApiConnection.QueryAsync<EditHistoryFieldChangeDto>(query);
    }
}