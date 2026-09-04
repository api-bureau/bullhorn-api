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

    private protected string RequestUrl { get; }

    private protected string DefaultFields { get; }

    public Task<T?> GetByIdAsync(int id, string? fields = null, CancellationToken cancellationToken = default)
        => _operations.GetByIdAsync(id, fields, cancellationToken);

    public Task<List<T>> GetByIdsAsync(IEnumerable<int> ids, string? fields = null, CancellationToken cancellationToken = default)
        => _operations.GetByIdsAsync(ids, fields, cancellationToken);

    public Task<List<T>> GetAddedSinceAsync(long timestampFrom, string? fields = null, CancellationToken cancellationToken = default)
        => _operations.GetAddedSinceAsync(timestampFrom, fields, cancellationToken);

    public Task<List<T>> GetAddedBetweenAsync(long timestampFrom, long timestampTo, string? fields = null, CancellationToken cancellationToken = default)
        => _operations.GetAddedBetweenAsync(timestampFrom, timestampTo, fields, cancellationToken);

    public Task<List<T>> GetChangedSinceAsync(long timestampFrom, string? fields = null, CancellationToken cancellationToken = default)
        => _operations.GetChangedSinceAsync(timestampFrom, fields, cancellationToken);

    public Task<List<T>> GetUpdatedSinceAsync(long timestampFrom, string? fields = null, CancellationToken cancellationToken = default)
        => _operations.GetUpdatedSinceAsync(timestampFrom, fields, cancellationToken);

    public Task<List<T>> GetWhereAsync(string? fields = null, string? defaultWhere = DefaultWhere, CancellationToken cancellationToken = default)
        => _operations.GetWhereAsync(fields, defaultWhere, cancellationToken);

    private protected Task<List<T>> ExecuteQueryAsync(string query, CancellationToken cancellationToken)
        => _operations.ExecuteAsync(query, cancellationToken);
}