namespace ApiBureau.Bullhorn.Api.Endpoints;

public class EntityEditHistory : BaseEndpoint
{
    public EntityEditHistory(ApiConnection apiConnection) : base(apiConnection) { }

    public async Task<List<EditHistoryFieldChangeDto>> GetAsync(EntityType entityType, long timestampFrom)
    {
        var query = $"{entityType}EditHistoryFieldChange?fields=id,display,columnName,newValue,oldValue,editHistory(dateAdded,modifyingPerson,targetEntity)&where=editHistory.dateAdded>{timestampFrom}";

        return await ApiConnection.QueryAsync<EditHistoryFieldChangeDto>(query);
    }
}
