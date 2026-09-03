using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ApiBureau.Bullhorn.Api.Http;

public sealed class BullhornHttpClient
{
    internal const int QueryCount = 500; // 500 max in BullhornApiJsonSerializerSettings
    private readonly HttpClient _client;
    private readonly ILogger<BullhornHttpClient> _logger;
    private readonly BullhornSettings _settings;
    private readonly ApiSession _session;
    private readonly TimeSpan _defaultTimeout = TimeSpan.FromMinutes(5);
    private int _apiCallCounter;
    public BullhornHttpClient(HttpClient client, IOptions<BullhornSettings> settings, ILogger<BullhornHttpClient> logger)
    {
        _client = client;
        _client.Timeout = _defaultTimeout;
        _logger = logger;
        _settings = settings.Value;
        _session = new ApiSession(_client, _settings, logger);

        CheckInitialisation();
    }

    private void CheckInitialisation()
    {
        if (string.IsNullOrEmpty(_settings.Secret) || string.IsNullOrEmpty(_settings.ClientId) || _settings.TokenUrl == null)
            _logger.LogError("BullhornSettings needs to be added and initialised Configuration.GetSection(nameof(BullhornSettings).");
    }

    //public void SetAuthorizationMeta(BullhornSettings bullhornSettings) => _settings = bullhornSettings;

    internal async Task<bool> CheckConnectionAsync(IProgress<string>? progress = null)
    {
        if (_settings is null)
        {
            _logger.LogError("Make sure you have got BullhornSettings in your appsettings.json.");

            throw new InvalidOperationException($"{nameof(BullhornSettings)}, Set the {nameof(BullhornSettings)} parameter before connecting!");
        }

        if (_session.IsValid)
        {
            progress?.Report("Bullhorn connection is established.");

            return true;
        }

        try
        {
            await _session.ConnectAsync(progress);

            return true;
        }
        catch (Exception ex)
        {
            progress?.Report($"Bullhorn connection failed: {ex.Message}");

            _logger.LogError(ex, "Connection failed after all retry attempts.");

            return false;
        }
    }

    internal async Task<HttpResponseMessage> ApiGetAsync(string query, int count, int start = 0, CancellationToken token = default)
    {
        query = $"{query}&start={start}&count={count}&showTotalMatched=true&usev2=true";

        return await GetAsync(query, token);
    }

    internal async Task<QueryResponse<T>?> QueryPageAsync<T>(string query, int count, int start = 0, CancellationToken token = default)
    {
        query = $"query/{query}&start={start}&count={count}&showTotalMatched=true&usev2=true";

        var response = await GetAsync(query, token);

        return await DeserializeAsync<QueryResponse<T>>(response);
    }

    /// <summary>
    /// Performs an asynchronous search query against the API and deserializes the response into a strongly-typed object.
    /// </summary>
    /// <typeparam name="T">The type of the expected result in the search response.</typeparam>
    /// <param name="searchTerm">The term or keyword to search for.</param>
    /// <param name="count">The number of results to return.</param>
    /// <param name="start">The starting index for paginated results (default is 0).</param>
    /// <returns>A task representing the asynchronous operation, containing the search response or <c>null</c> if the response couldn't be deserialized.</returns>
    internal async Task<SearchResponse<T>?> SearchPageAsync<T>(string searchTerm, int count, int start = 0, CancellationToken token = default)
    {
        var query = $"search/{searchTerm}&start={start}&count={count}&showTotalMatched=true&usev2=true";

        var response = await GetAsync(query, token);

        return await DeserializeAsync<SearchResponse<T>>(response);
    }

    internal async Task<HttpResponseMessage> GetAsync(string query, CancellationToken token)
    {
        await PingCheckAsync(token);

        var restUrl = $"{_session.LoginResponse!.RestUrl}{query}";

        return await _client.GetAsync(restUrl, token);
    }

    // This might be wrapped to ApiCreateEntity
    internal async Task<HttpResponseMessage> ApiPutAsync(string query, HttpContent content, CancellationToken token)
    {
        await PingCheckAsync(token);

        var restUrl = $"{_session.LoginResponse!.RestUrl}{query}";

        return await _client.PutAsync(restUrl, content);
    }

    internal async Task<Result<ChangeResponse>> PutAsJsonAsync(EntityType type, object content, CancellationToken token)
    {
        var response = await PutAsJsonAsync($"entity/{type}", content, token);

        return await GetChangeResponseAsync(response).ConfigureAwait(false);
    }

    internal async Task<HttpResponseMessage> PutAsJsonAsync(string query, object content, CancellationToken token)
    {
        await PingCheckAsync(token);

        var restUrl = $"{_session.LoginResponse!.RestUrl}{query}";

        try
        {
            return await _client.PutAsJsonAsync(restUrl, content, token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PutAsJsonAsync");
        }

        return new HttpResponseMessage();
    }

    // Probably this pattern should be used across
    internal async Task<Result<ChangeResponse>> PostAsJsonAsync(EntityType type, int entityId, object content, CancellationToken token = default)
    {
        var response = await PostAsJsonAsync($"entity/{type}/{entityId}", content, token);

        return await GetChangeResponseAsync(response).ConfigureAwait(false);
    }

    internal async Task<HttpResponseMessage> PostAsJsonAsync(string query, object content, CancellationToken token = default)
    {
        await PingCheckAsync(token);

        var restUrl = $"{_session.LoginResponse!.RestUrl}{query}";

        try
        {
            return await _client.PostAsJsonAsync(restUrl, content, token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PostAsJsonAsync");
        }

        return new HttpResponseMessage();
    }

    internal async Task<HttpResponseMessage> PostAsync(string query, HttpContent? content, CancellationToken token)
    {
        await PingCheckAsync(token);

        var restUrl = $"{_session.LoginResponse!.RestUrl}{query}";

        try
        {
            return await _client.PostAsync(restUrl, content, token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PostAsync");
        }

        return new HttpResponseMessage();
    }

    internal async Task UpdateAsync<T>(int id, string entityName, T updateDto, CancellationToken token) => await PostAsync($"entity/{entityName}/{id}",
            new StringContent(JsonSerializer.Serialize(updateDto, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }), Encoding.UTF8, "application/json"), token);

    internal async Task MassUpdateAsync<T>(string entityName, T updateDto, CancellationToken token) => await PostAsync($"massUpdate/{entityName}?",
            new StringContent(JsonSerializer.Serialize(updateDto, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Always
            }), Encoding.UTF8, "application/json"), token);

    internal async Task<Result<ChangeResponse>> DeleteAsync(int id, EntityType type, CancellationToken token)
    {
        var response = await ApiDeleteAsync($"entity/{type}/{id}?", token);

        return await GetChangeResponseAsync(response).ConfigureAwait(false);
    }

    internal async Task<HttpResponseMessage> ApiDeleteAsync(string query, CancellationToken token)
    {
        await PingCheckAsync(token);

        var restUrl = $"{_session.LoginResponse!.RestUrl}{query}";

        return await _client.DeleteAsync(restUrl, token);
    }

    internal Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
        => response.DeserializeAsync<T>(_logger);

    internal void LogWarning(string text) => _logger.LogWarning(text);

    internal void LogError(string message, params object?[] args) => _logger.LogError(message, args);

    private async Task PingCheckAsync(CancellationToken token)
    {
        _logger.LogDebug("Next token refresh at {expiryDate}", _session.Ping.SessionExpiryDate);

        if (!_session.IsValid)
        {
            _logger.LogError("{0}, Not logged in yet.", nameof(PingCheckAsync));
        }

        // A still-valid ping means the existing server session can be reused.
        if (_session.Ping?.Valid ?? false) return;

        _apiCallCounter++;

        try
        {
            using var response = await _client.GetAsync($"{_session.LoginResponse!.RestUrl}/ping", token);

            var result = await DeserializeAsync<PingResponse>(response);

            if (result is null)
            {
                _logger.LogError("PingCheckAsync, Response deserialization failed.");

                return;
            }

            _session.Ping = result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PingCheckAsync");

            // Reconnect before checking whether a token refresh is still required.
            await _session.ConnectAsync(token: token);
        }

        if (_session.Ping is null)
        {
            _logger.LogError("PingCheckAsync, Ping is null.");

            return;
        }

        _logger.LogDebug("Next token refresh at {0}", _session.Ping.SessionExpiryDate);

        if (_session.Ping.Valid) return;

        // Refresh only when both the ping and reconnect paths leave the session invalid.
        _logger.LogInformation($"Token refresh on {_apiCallCounter} API call.");

        await _session.RefreshTokenAsync(token);
    }

    private static async Task<Result<ChangeResponse>> GetChangeResponseAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            //var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            var errorResponse = await response.Content.ReadFromJsonAsync<ChangeResponse>();

            if (errorResponse is null)
            {
                return Result.Failure<ChangeResponse>(response.ReasonPhrase);
            }

            return errorResponse;
        }

        try
        {
            var changeResponse = await response.Content.ReadFromJsonAsync<ChangeResponse>();

            if (changeResponse is null)
            {
                return Result.Failure<ChangeResponse>("Response deserialization failed.");
            }

            return Result.Success(changeResponse);
        }
        catch (Exception e)
        {
            return Result.Failure<ChangeResponse>(e.Message);
        }
    }
}
// Other query examples
//search/Note?fields=id,dateAdded,action,commentingPerson&query=dateAdded:[20210101000000 TO *] AND action:'Phone Call'&sort=-dateAdded
//[ContestType.GdprWithDrawn] = "Candidate?fields=id&query=notes.id:\"^^action:(\\\"gdpr withdrawn\\\") AND isDeleted:false\""
//            var query = "Candidate?fields=id,status,firstName,dateAdded,owner,email,email2,email3,phone,phone2,phone3,mobile,workPhone,placements[0](id),sendouts(id,dateAdded),fileAttachments(id)&query=isDeleted:0 AND -status:\"Archive\" AND -email:[\"\" TO *] AND -email2:[\"\" TO *] AND -email3:[\"\" TO *] AND -mobile:[\"\" TO *] AND -phone:[\"\" TO *] AND -placements.id:[0 TO 99999999999] AND -interviews.id:[0 TO 99999999999] AND -fileAttachments.id:[0 TO 99999999999] AND -notes.id:\"^^action:(\\\"Parse Failed Remove\\\") AND isDeleted:false\" AND -notes.id:\"^^action:(\\\"Parse Failed Keep\\\") AND isDeleted:false\" &sort=-dateAdded";
//            var query = "Candidate?fields=id,status,email,email2,email3,mobile,phone,notes(id,action)&query=isDeleted:0 AND notes.id:\"^^action:(\\\"No Files Remove\\\") AND isDeleted:false\" AND (email:[\"\" TO *] OR email2:[\"\" TO *] OR email3:[\"\" TO *] OR mobile:[\"\" TO *]  OR phone:[\"\" TO *] ) AND -phone:\"44 01700000000\"
