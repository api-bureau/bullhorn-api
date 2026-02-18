namespace ApiBureau.Bullhorn.Api.Endpoints;

public abstract class QueryBaseEndpoint<T> : EntityEndpointBase<T>
{
    private const string DefaultWhere = "id>0";

    public QueryBaseEndpoint(ApiConnection apiConnection, string requestUrl, string defaultFields) : base(apiConnection, requestUrl, defaultFields) { }

    public async Task<List<T>> QueryFromAsync(long timestampFrom, string? fields = null, CancellationToken token = default)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&where=dateAdded>={timestampFrom}";

        // Appointments "AND candidateReference IS NOT NULL"

        return await ApiConnection.QueryAsync<T>(query, token).ConfigureAwait(false);
    }

    public async Task<List<T>> QueryFromToAsync(long timestampFrom, long timestampTo, string? fields = null, CancellationToken token = default)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&where=dateAdded>={timestampFrom} AND dateAdded<{timestampTo}";

        // Appointments "AND candidateReference IS NOT NULL"

        return await ApiConnection.QueryAsync<T>(query, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Uses /query, and dateAdded and dateLastModified for search
    /// </summary>
    /// <param name="timestampFrom"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public async Task<List<T>> QueryNewAndUpdatedFromAsync(long timestampFrom, string? fields = null, CancellationToken token = default)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&where=dateAdded>={timestampFrom} OR dateLastModified>={timestampFrom}";

        // Appointments "AND candidateReference IS NOT NULL"

        return await ApiConnection.QueryAsync<T>(query, token).ConfigureAwait(false);
    }

    public async Task<List<T>> QueryUpdatedFromAsync(long timestampFrom, string? fields = null, CancellationToken token = default)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&where=dateLastModified>{timestampFrom}";

        return await ApiConnection.QueryAsync<T>(query, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns all items, default where condition is "id>0", useful e.g. for returing all Departments and Countries
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="defaultWhere"></param>
    /// <returns></returns>
    public async Task<List<T>> QueryWhereAsync(string? fields = null, string? defaultWhere = DefaultWhere, CancellationToken token = default)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&where={defaultWhere ?? DefaultWhere}";

        return await ApiConnection.QueryAsync<T>(query, token).ConfigureAwait(false);
    }
}