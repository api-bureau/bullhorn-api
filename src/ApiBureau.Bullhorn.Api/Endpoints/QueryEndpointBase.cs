using ApiBureau.Bullhorn.Api.Internals;

namespace ApiBureau.Bullhorn.Api.Endpoints;

public abstract class QueryEndpointBase<T>
{
    private const string DefaultWhere = "id>0";
    private readonly QueryOperations<T> _operations;

    private protected QueryEndpointBase(BullhornHttpClient httpClient, string requestUrl, string defaultFields)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        HttpClient = httpClient;
        RequestUrl = requestUrl;
        DefaultFields = defaultFields;
        _operations = new(httpClient, requestUrl, defaultFields);
    }

    private protected BullhornHttpClient HttpClient { get; }

    public string RequestUrl { get; }

    public string DefaultFields { get; }

    // ToDo rename to GetByIdAsync during public API normalization.
    public Task<T?> GetAsync(int id, string? fields = null, CancellationToken token = default)
        => _operations.GetAsync(id, fields, token);

    // ToDo rename to GetByIdsAsync during public API normalization.
    public Task<List<T>> GetAsync(IEnumerable<int> ids, string? fields = null, CancellationToken token = default)
        => _operations.GetAsync(ids, fields, token);

    // ToDo rename to GetAddedSinceAsync during public API normalization.
    public Task<List<T>> QueryFromAsync(long timestampFrom, string? fields = null, CancellationToken token = default)
        => _operations.QueryFromAsync(timestampFrom, fields, token);

    // ToDo rename to GetAddedBetweenAsync during public API normalization.
    public Task<List<T>> QueryFromToAsync(long timestampFrom, long timestampTo, string? fields = null, CancellationToken token = default)
        => _operations.QueryFromToAsync(timestampFrom, timestampTo, fields, token);

    // ToDo rename to GetChangedSinceAsync during public API normalization.
    public Task<List<T>> QueryNewAndUpdatedFromAsync(long timestampFrom, string? fields = null, CancellationToken token = default)
        => _operations.QueryNewAndUpdatedFromAsync(timestampFrom, fields, token);

    // ToDo rename to GetUpdatedSinceAsync during public API normalization.
    public Task<List<T>> QueryUpdatedFromAsync(long timestampFrom, string? fields = null, CancellationToken token = default)
        => _operations.QueryUpdatedFromAsync(timestampFrom, fields, token);

    // ToDo rename to GetWhereAsync during public API normalization.
    public Task<List<T>> QueryWhereAsync(string? fields = null, string? defaultWhere = DefaultWhere, CancellationToken token = default)
        => _operations.QueryWhereAsync(fields, defaultWhere, token);

    private protected Task<List<T>> ExecuteQueryAsync(string query, CancellationToken token)
        => _operations.ExecuteAsync(query, token);
}