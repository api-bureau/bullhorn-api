namespace ApiBureau.Bullhorn.Api.Endpoints;

public class SearchBaseEndpoint<T> : BaseEntityEndpoint<T>
{
    public SearchBaseEndpoint(ApiConnection apiConnection, string requestUrl, string defaultFields) : base(apiConnection, requestUrl, defaultFields) { }

    /// <summary>
    /// Uses /search, and dateAdded for search
    /// </summary>
    /// <param name="dateTimeFrom"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public async Task<List<T>> SearchFromAsync(DateTime dateTimeFrom, string? fields = null)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO *]";

        return await ApiConnection.SearchAsync<T>(query).ConfigureAwait(false);
    }

    /// <summary>
    /// Uses /search, and dateAdded for search
    /// </summary>
    /// <param name="dateTimeFrom"></param>
    /// <param name="dateTimeTo"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public async Task<List<T>> SearchFromToAsync(DateTime dateTimeFrom, DateTime dateTimeTo, string? fields = null)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO {dateTimeTo:yyyyMMddHHmmss}]";

        return await ApiConnection.SearchAsync<T>(query).ConfigureAwait(false);
    }

    /// <summary>
    /// Uses /search, and dateAdded and dateLastModified for search
    /// </summary>
    /// <param name="dateTimeFrom"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public async Task<List<T>> SearchNewAndUpdatedFromAsync(DateTime dateTimeFrom, string? fields = null)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO *] OR dateLastModified:[{dateTimeFrom:yyyyMMddHHmmss} TO *]";

        return await ApiConnection.SearchAsync<T>(query).ConfigureAwait(false);
    }

    public async Task<List<T>> GetUpdatedFromAsync(DateTime dateTime, string? fields = null)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&query=dateLastModified:[{dateTime:yyyyMMddHHmmss} TO *]";

        return await ApiConnection.SearchAsync<T>(query).ConfigureAwait(false);
    }
}

public class QueryBaseEndpoint<T> : BaseEntityEndpoint<T>
{
    private const string DefaultWhere = "id>0";

    public QueryBaseEndpoint(ApiConnection apiConnection, string requestUrl, string defaultFields) : base(apiConnection, requestUrl, defaultFields) { }

    public async Task<List<T>> QueryFromAsync(long timestampFrom, string? fields = null)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&where=dateAdded>={timestampFrom}";

        // Appoitments "AND candidateReference IS NOT NULL"

        return await ApiConnection.QueryAsync<T>(query).ConfigureAwait(false);
    }

    public async Task<List<T>> QueryFromToAsync(long timestampFrom, long timestampTo, string? fields = null)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&where=dateAdded>={timestampFrom} AND dateAdded<{timestampTo}";

        // Appoitments "AND candidateReference IS NOT NULL"

        return await ApiConnection.QueryAsync<T>(query).ConfigureAwait(false);
    }

    /// <summary>
    /// Uses /query, and dateAdded and dateLastModified for search
    /// </summary>
    /// <param name="timestampFrom"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public async Task<List<T>> QueryNewAndUpdatedFromAsync(long timestampFrom, string? fields = null)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&where=dateAdded>={timestampFrom} OR dateLastModified>={timestampFrom}";

        // Appoitments "AND candidateReference IS NOT NULL"

        return await ApiConnection.QueryAsync<T>(query).ConfigureAwait(false);
    }

    public async Task<List<T>> QueryUpdatedFromAsync(long timestampFrom, string? fields = null)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&where=dateLastModified>{timestampFrom}";

        return await ApiConnection.QueryAsync<T>(query).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns all items, default where condition is "id>0", useful e.g. for returing all Departments and Countries
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="defaultWhere"></param>
    /// <returns></returns>
    public async Task<List<T>> QueryWhereAsync(string? fields = null, string? defaultWhere = DefaultWhere)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&where={defaultWhere ?? DefaultWhere}";

        return await ApiConnection.QueryAsync<T>(query).ConfigureAwait(false);
    }
}

public class BaseEntityEndpoint<T> : BaseEndpoint
{
    public string DefaultFields { get; }

    public BaseEntityEndpoint(ApiConnection apiConnection, string requestUrl, string defaultFields) : base(apiConnection, requestUrl)
        => DefaultFields = defaultFields;

    /// <summary>
    /// uses /entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public async Task<T> GetAsync(int id, string? fields = null)
    {
        var query = $"{RequestUrl}/{id}?fields={fields ?? DefaultFields}";

        return await ApiConnection.EntityAsync<T>(query).ConfigureAwait(false);
    }

    /// <summary>
    /// uses /entity
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public async Task<List<T>> GetAsync(IEnumerable<int> ids, string? fields = null)
    {
        if (ids.Count() == 1)
            return new List<T> { await GetAsync(ids.First(), fields).ConfigureAwait(false) };

        var query = $"{RequestUrl}/{string.Join(",", ids)}?fields={fields ?? DefaultFields}";

        return await ApiConnection.EntityAsync<List<T>>(query).ConfigureAwait(false);
    }
}

public class BaseEndpoint
{
    protected ApiConnection ApiConnection { get; }
    public string RequestUrl { get; }

    public BaseEndpoint(ApiConnection apiConnection, string requestUrl)
    {
        ApiConnection = apiConnection;
        RequestUrl = requestUrl;
    }
}