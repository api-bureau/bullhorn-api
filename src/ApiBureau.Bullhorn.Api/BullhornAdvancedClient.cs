using ApiBureau.Bullhorn.Api.Internals;

namespace ApiBureau.Bullhorn.Api;

public sealed class BullhornAdvancedClient
{
    private readonly BullhornHttpClient _httpClient;

    internal BullhornAdvancedClient(BullhornHttpClient httpClient)
        => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>Verifies the session with Bullhorn, recovering it if rejected.</summary>
    public Task VerifyConnectionAsync(CancellationToken cancellationToken = default)
        => _httpClient.VerifyConnectionAsync(cancellationToken);

    /// <summary>Forces full authorization and verifies the new REST session.</summary>
    public Task ReconnectAsync(IProgress<string>? progress = null, CancellationToken cancellationToken = default)
        => _httpClient.ReconnectAsync(progress, cancellationToken);

    /// <summary>
    /// Corrupts the cached token for all consumers of this client, to test recovery.
    /// Does not revoke the server session. The next request uses an invalid token.
    /// </summary>
    public Task InvalidateSessionForTestingAsync(CancellationToken cancellationToken = default)
        => _httpClient.InvalidateSessionForTestingAsync(cancellationToken);

    public Task<List<T>> QueryAsync<T>(string query, CancellationToken cancellationToken = default)
        => new QueryOperations<T>(_httpClient, string.Empty, string.Empty).ExecuteAsync(query, cancellationToken);

    public Task<HttpResponseMessage> GetRawPageAsync(
        string query,
        int count,
        int start = 0,
        CancellationToken cancellationToken = default)
        => _httpClient.GetRawPageAsync(query, count, start, cancellationToken);
}