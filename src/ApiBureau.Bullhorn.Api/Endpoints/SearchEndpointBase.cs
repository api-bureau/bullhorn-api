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

    public string RequestUrl { get; }

    public string DefaultFields { get; }

    // ToDo rename to GetByIdAsync during public API normalization.
    public Task<T?> GetAsync(int id, string? fields = null, CancellationToken token = default)
        => _operations.GetAsync(id, fields, token);

    // ToDo rename to GetByIdsAsync during public API normalization.
    public Task<List<T>> GetAsync(IEnumerable<int> ids, string? fields = null, CancellationToken token = default)
        => _operations.GetAsync(ids, fields, token);

    // ToDo rename to GetAddedSinceAsync during public API normalization.
    public Task<List<T>> SearchFromAsync(DateTime dateTimeFrom, string? fields = null, CancellationToken token = default)
        => _operations.SearchFromAsync(dateTimeFrom, fields, token);

    // ToDo rename to GetAddedBetweenAsync during public API normalization.
    public Task<List<T>> SearchFromToAsync(DateTime dateTimeFrom, DateTime dateTimeTo, string? fields = null, CancellationToken token = default)
        => _operations.SearchFromToAsync(dateTimeFrom, dateTimeTo, fields, token);

    // ToDo rename to GetChangedSinceAsync during public API normalization.
    public Task<List<T>> SearchNewAndUpdatedFromAsync(DateTime dateTimeFrom, string? fields = null, CancellationToken token = default)
        => _operations.SearchNewAndUpdatedFromAsync(dateTimeFrom, fields, token);

    // ToDo rename to GetUpdatedSinceAsync during public API normalization.
    public Task<List<T>> GetUpdatedFromAsync(DateTime dateTime, string? fields = null, CancellationToken token = default)
        => _operations.GetUpdatedFromAsync(dateTime, fields, token);

    private protected Task<List<T>> ExecuteSearchAsync(string searchTerm, CancellationToken token = default)
        => _operations.ExecuteAsync(searchTerm, token: token);
}