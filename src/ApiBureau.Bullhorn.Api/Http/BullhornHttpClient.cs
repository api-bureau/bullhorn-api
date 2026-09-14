using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace ApiBureau.Bullhorn.Api.Http;

public sealed class BullhornHttpClient
{
    internal const int QueryCount = 500; // 500 max in BullhornApiJsonSerializerSettings
    private static readonly JsonSerializerOptions ResponseJsonOptions = new(JsonSerializerDefaults.Web)
    {
        AllowTrailingCommas = true
    };
    private readonly HttpClient _client;
    private readonly ILogger<BullhornHttpClient> _logger;
    private readonly BullhornSettings _settings;
    private readonly TimeSpan _defaultTimeout = TimeSpan.FromMinutes(5);
    private readonly BullhornSessionManager _sessions;
    public BullhornHttpClient(HttpClient client, IOptions<BullhornSettings> settings, ILogger<BullhornHttpClient> logger)
    {
        _client = client;
        _client.Timeout = _defaultTimeout;
        _logger = logger;
        _settings = settings.Value;
        _sessions = new BullhornSessionManager(_client, new ApiSession(_client, _settings), logger, _settings.SessionVerificationInterval);

        if (string.IsNullOrEmpty(_settings.Secret) || string.IsNullOrEmpty(_settings.ClientId) || _settings.TokenUrl == null)
            _logger.LogError("BullhornSettings needs to be added and initialised Configuration.GetSection(nameof(BullhornSettings).");
    }

    //public void SetAuthorizationMeta(BullhornSettings bullhornSettings) => _settings = bullhornSettings;

    internal async Task<bool> CheckConnectionAsync(IProgress<string>? progress = null)
    {
        try
        {
            await _sessions.EnsureAsync(CancellationToken.None, progress: progress);

            return true;
        }
        catch (Exception ex)
        {
            progress?.Report($"Bullhorn connection failed: {ex.Message}");

            _logger.LogError(ex, "Bullhorn connection check failed.");

            return false;
        }
    }

    internal Task ReconnectAsync(IProgress<string>? progress, CancellationToken token)
        => _sessions.ReconnectAsync(progress, token);

    internal Task VerifyConnectionAsync(CancellationToken token)
        => _sessions.EnsureAsync(token, verify: true);

    internal Task InvalidateSessionForTestingAsync(CancellationToken token)
        => _sessions.InvalidateForTestingAsync(token);

    internal async Task<HttpResponseMessage> GetRawPageAsync(string query, int count, int start = 0, CancellationToken cancellationToken = default)
    {
        query = $"{query}&start={start}&count={count}&showTotalMatched=true&usev2=true";

        return await GetAsync(query, cancellationToken);
    }

    internal async Task<QueryResponse<T>?> QueryPageAsync<T>(string query, int count, int start = 0, CancellationToken cancellationToken = default)
    {
        query = $"query/{query}&start={start}&count={count}&showTotalMatched=true&usev2=true";

        using var response = await GetAsync(query, cancellationToken);

        return await BullhornResponseReader.ReadPageAsync<QueryResponse<T>>(response, cancellationToken);
    }

    /// <summary>
    /// Performs an asynchronous search query against the API and deserializes the response into a strongly-typed
    /// object.
    /// </summary>
    /// <typeparam name="T">The type of the expected result in the search response.</typeparam>
    /// <param name="searchTerm">The term or keyword to search for.</param>
    /// <param name="count">The number of results to return.</param>
    /// <param name="start">The starting index for paginated results (default is 0).</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the search response or <c>null</c> if the response
    /// couldn't be deserialized.
    /// </returns>
    internal async Task<SearchResponse<T>?> SearchPageAsync<T>(string searchTerm, int count, int start = 0, CancellationToken cancellationToken = default)
    {
        var query = $"search/{searchTerm}&start={start}&count={count}&showTotalMatched=true&usev2=true";

        using var response = await GetAsync(query, cancellationToken);

        return await BullhornResponseReader.ReadPageAsync<SearchResponse<T>>(response, cancellationToken);
    }

    internal async Task<HttpResponseMessage> GetAsync(string query, CancellationToken cancellationToken)
    {
        var session = await _sessions.EnsureAsync(cancellationToken);
        var response = await SendAsync(session, HttpMethod.Get, query, null, cancellationToken);

        if (!await BullhornSessionManager.IsSessionRejectedAsync(response, cancellationToken)) return response;

        response.Dispose();

        session = await _sessions.RecoverAsync(session, cancellationToken);

        // A read is replayed at most once. Persistent rejection reaches the caller.
        response = await SendAsync(session, HttpMethod.Get, query, null, cancellationToken);

        if (await BullhornSessionManager.IsSessionRejectedAsync(response, cancellationToken))
            await _sessions.RejectAsync(session, cancellationToken);

        return response;
    }

    // This might be wrapped to ApiCreateEntity
    internal async Task<HttpResponseMessage> ApiPutAsync(string query, HttpContent content, CancellationToken cancellationToken)
    {
        return await SendWriteAsync(HttpMethod.Put, query, content, cancellationToken);
    }

    internal async Task<Result<ChangeResponse, ErrorResponse>> PutAsJsonAsync(EntityType type, object content, CancellationToken cancellationToken)
    {
        using var json = JsonContent.Create(content);
        using var response = await SendWriteAsync(HttpMethod.Put, $"entity/{type}", json, cancellationToken);

        return await BullhornResponseReader.ReadResultAsync<ChangeResponse>(response).ConfigureAwait(false);
    }

    // Probably this pattern should be used across
    internal async Task<Result<ChangeResponse, ErrorResponse>> PostAsJsonAsync(EntityType type, int entityId, object content, CancellationToken cancellationToken = default)
    {
        using var response = await PostAsJsonAsync($"entity/{type}/{entityId}", content, cancellationToken);

        return await BullhornResponseReader.ReadResultAsync<ChangeResponse>(response).ConfigureAwait(false);
    }

    internal async Task<HttpResponseMessage> PostAsJsonAsync(string query, object content, CancellationToken cancellationToken = default)
    {
        using var json = JsonContent.Create(content);

        return await SendWriteAsync(HttpMethod.Post, query, json, cancellationToken);
    }

    internal async Task<HttpResponseMessage> PostAsync(string query, HttpContent? content, CancellationToken cancellationToken)
    {
        return await SendWriteAsync(HttpMethod.Post, query, content, cancellationToken);
    }

    //internal async Task UpdateAsync<T>(int id, string entityName, T updateDto, CancellationToken cancellationToken) => await PostAsync($"entity/{entityName}/{id}",
    //        new StringContent(JsonSerializer.Serialize(updateDto, new JsonSerializerOptions
    //        {
    //            AllowTrailingCommas = true,
    //            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    //        }), Encoding.UTF8, "application/json"), cancellationToken);

    //internal async Task MassUpdateAsync<T>(string entityName, T updateDto, CancellationToken cancellationToken) => await PostAsync($"massUpdate/{entityName}?",
    //        new StringContent(JsonSerializer.Serialize(updateDto, new JsonSerializerOptions
    //        {
    //            AllowTrailingCommas = true,
    //            DefaultIgnoreCondition = JsonIgnoreCondition.Always
    //        }), Encoding.UTF8, "application/json"), cancellationToken);

    internal async Task<Result<ChangeResponse, ErrorResponse>> DeleteAsync(int id, EntityType type, CancellationToken cancellationToken)
    {
        using var response = await ApiDeleteAsync($"entity/{type}/{id}?", cancellationToken);

        return await BullhornResponseReader.ReadResultAsync<ChangeResponse>(response).ConfigureAwait(false);
    }

    internal async Task<HttpResponseMessage> ApiDeleteAsync(string query, CancellationToken cancellationToken)
    {
        return await SendWriteAsync(HttpMethod.Delete, query, null, cancellationToken);
    }

    internal async Task<T?> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await BullhornResponseReader.ReadErrorAsync(response, cancellationToken);

            throw new HttpRequestException(error.Message, null, response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<T>(ResponseJsonOptions, cancellationToken)
            ?? throw new HttpRequestException("Bullhorn returned an empty response.");
    }

    internal void LogWarning(string text) => _logger.LogWarning(text);

    internal void LogError(string message, params object?[] args) => _logger.LogError(message, args);

    private async Task<HttpResponseMessage> SendWriteAsync(HttpMethod method, string path, HttpContent? content, CancellationToken token)
    {
        var session = await _sessions.EnsureAsync(token);
        var response = await SendAsync(session, method, path, content, token);

        if (await BullhornSessionManager.IsSessionRejectedAsync(response, token))
        {
            // Repair the connection for subsequent calls, but never replay a mutation.
            try { await _sessions.RecoverAsync(session, token); }
            catch { response.Dispose(); throw; }
        }

        return response;
    }

    private async Task<HttpResponseMessage> SendAsync(BullhornSession session, HttpMethod method,
        string path, HttpContent? content, CancellationToken token)
    {
        using var request = BullhornSessionManager.CreateRequest(session, method, path);

        request.Content = content;

        try { return await _client.SendAsync(request, token); }
        finally { request.Content = null; } // Content remains owned by the caller.
    }

}
// Other query examples
//search/Note?fields=id,dateAdded,action,commentingPerson&query=dateAdded:[20210101000000 TO *] AND action:'Phone Call'&sort=-dateAdded
//[ContestType.GdprWithDrawn] = "Candidate?fields=id&query=notes.id:\"^^action:(\\\"gdpr withdrawn\\\") AND isDeleted:false\""
//            var query = "Candidate?fields=id,status,firstName,dateAdded,owner,email,email2,email3,phone,phone2,phone3,mobile,workPhone,placements[0](id),sendouts(id,dateAdded),fileAttachments(id)&query=isDeleted:0 AND -status:\"Archive\" AND -email:[\"\" TO *] AND -email2:[\"\" TO *] AND -email3:[\"\" TO *] AND -mobile:[\"\" TO *] AND -phone:[\"\" TO *] AND -placements.id:[0 TO 99999999999] AND -interviews.id:[0 TO 99999999999] AND -fileAttachments.id:[0 TO 99999999999] AND -notes.id:\"^^action:(\\\"Parse Failed Remove\\\") AND isDeleted:false\" AND -notes.id:\"^^action:(\\\"Parse Failed Keep\\\") AND isDeleted:false\" &sort=-dateAdded";
//            var query = "Candidate?fields=id,status,email,email2,email3,mobile,phone,notes(id,action)&query=isDeleted:0 AND notes.id:\"^^action:(\\\"No Files Remove\\\") AND isDeleted:false\" AND (email:[\"\" TO *] OR email2:[\"\" TO *] OR email3:[\"\" TO *] OR mobile:[\"\" TO *]  OR phone:[\"\" TO *] ) AND -phone:\"44 01700000000\"