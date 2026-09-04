using ApiBureau.Bullhorn.Api.Internals;

namespace ApiBureau.Bullhorn.Api;

public sealed class BullhornAdvancedClient
{
    private readonly BullhornHttpClient _httpClient;

    internal BullhornAdvancedClient(BullhornHttpClient httpClient)
        => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public Task<List<T>> QueryAsync<T>(string query, CancellationToken cancellationToken = default)
        => new QueryOperations<T>(_httpClient, string.Empty, string.Empty).ExecuteAsync(query, cancellationToken);

    public Task<HttpResponseMessage> GetRawPageAsync(
        string query,
        int count,
        int start = 0,
        CancellationToken cancellationToken = default)
        => _httpClient.GetRawPageAsync(query, count, start, cancellationToken);
}