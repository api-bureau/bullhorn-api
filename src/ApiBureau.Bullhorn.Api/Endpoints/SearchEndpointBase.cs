using ApiBureau.Bullhorn.Api.Internals;

namespace ApiBureau.Bullhorn.Api.Endpoints;

public abstract class SearchEndpointBase<T>
{
    private readonly SearchOperations<T> _operations;

    private protected SearchEndpointBase(BullhornHttpClient httpClient, string requestUrl, string defaultFields)
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

    public Task<List<T>> GetAddedSinceAsync(DateTime dateTimeFrom, string? fields = null, CancellationToken cancellationToken = default)
        => _operations.GetAddedSinceAsync(dateTimeFrom, fields, cancellationToken);

    public Task<List<T>> GetAddedBetweenAsync(DateTime dateTimeFrom, DateTime dateTimeTo, string? fields = null, CancellationToken cancellationToken = default)
        => _operations.GetAddedBetweenAsync(dateTimeFrom, dateTimeTo, fields, cancellationToken);

    public Task<List<T>> GetChangedSinceAsync(DateTime dateTimeFrom, string? fields = null, CancellationToken cancellationToken = default)
        => _operations.GetChangedSinceAsync(dateTimeFrom, fields, cancellationToken);

    public Task<List<T>> GetUpdatedSinceAsync(DateTime dateTime, string? fields = null, CancellationToken cancellationToken = default)
        => _operations.GetUpdatedSinceAsync(dateTime, fields, cancellationToken);

    private protected Task<List<T>> ExecuteSearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        => _operations.ExecuteAsync(searchTerm, cancellationToken: cancellationToken);
}