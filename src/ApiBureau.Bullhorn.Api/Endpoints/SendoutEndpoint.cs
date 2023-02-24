namespace ApiBureau.Bullhorn.Api.Endpoints;

public class SendoutEndpoint : QueryBaseEndpoint<SendoutDto>
{
    private const string EntityDefaultFields = "id,candidate,user,dateAdded,jobOrder,clientContact,clientCorporation";

    public SendoutEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Get all from a specific timestamp
    /// </summary>
    /// <param name="timestampFrom"></param>
    /// <returns></returns>
    public async Task<List<SendoutDto>> GetAsync(long timestampFrom)
    {
        var query = $"Sendout?fields=id,candidate,user,dateAdded,jobOrder,clientContact,clientCorporation&where=dateAdded>={timestampFrom}";

        return await ApiConnection.QueryAsync<SendoutDto>(query);
    }
}
