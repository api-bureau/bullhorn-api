using ApiBureau.Bullhorn.Api.Dtos;

namespace ApiBureau.Bullhorn.Api.Endpoints.Entities
{
    public class EntityEditHistory
    {
        private readonly BullhornClient _bullhornApi;

        public EntityEditHistory(BullhornClient bullhornApi)
        {
            _bullhornApi = bullhornApi;
        }

        public async Task<List<EditHistoryFieldChangeDto>> GetAsync(EntityType entityType, long timestampFrom)
        {
            var query = $"{entityType}EditHistoryFieldChange?fields=id,display,columnName,newValue,oldValue,editHistory(dateAdded,modifyingPerson,targetEntity)&where=editHistory.dateAdded>{timestampFrom}";

            return await _bullhornApi.QueryAsync<EditHistoryFieldChangeDto>(query);
        }
    }
}
