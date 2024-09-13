using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ApiBureau.Bullhorn.Api.Http;

// Refactor according this 11th Minute https://channel9.msdn.com/Shows/XamarinShow/Azure-Active-Directory-B2C-Authentication-For-Mobile-with-Matthew-Soucoup
// Call it Bullhorn.Identity, AcquireTokenAsync

public class ApiConnection
{
    private const int QueryCount = 500; // 500 max in BullhornApiJsonSerializerSettings
    private readonly HttpClient _client;
    private readonly ILogger<ApiConnection> _logger;
    private readonly BullhornSettings _settings;
    private readonly ApiSession _session;
    private readonly TimeSpan _defaultTimeout = TimeSpan.FromMinutes(5);
    private int _apiCallCounter;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public ApiConnection(HttpClient client, IOptions<BullhornSettings> settings, ILogger<ApiConnection> logger)
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

    public async Task CheckConnectionAsync()
    {
        if (_settings is null)
        {
            _logger.LogError("Make sure you have got BullhornSettings in your appsettings.json.");

            throw new InvalidOperationException($"{nameof(BullhornSettings)}, Set the {nameof(BullhornSettings)} parameter before connecting!");
        }

        if (_session.IsValid) return;

        await _session.ConnectAsync();
    }

    public async Task<HttpResponseMessage> ApiGetAsync(string query, int count, int start = 0)
    {
        query = $"{query}&start={start}&count={count}&showTotalMatched=true&usev2=true";

        return await GetAsync(query);
    }

    /// <summary>
    /// Where condition must be included
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <returns></returns>
    public async Task<List<T>> QueryAsync<T>(string query)
    {
        var items = new List<T>();

        var result = await ApiQueryAsync<T>(query, QueryCount);

        items.AddRange(result?.Data ?? new());

        if (result is null) return items;

        for (var i = result.Count; i < result.Total; i += result!.Count)
        {
            result = await ApiQueryAsync<T>(query, QueryCount, i);

            items.AddRange(result?.Data ?? new());
        }

        return items;
    }

    public async Task<QueryResponse<T>?> ApiQueryAsync<T>(string query, int count, int start = 0)
    {
        query = $"query/{query}&start={start}&count={count}&showTotalMatched=true&usev2=true";

        var response = await GetAsync(query);

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
    public async Task<SearchResponse<T>?> SearchApiAsync<T>(string searchTerm, int count, int start = 0)
    {
        var query = $"search/{searchTerm}&start={start}&count={count}&showTotalMatched=true&usev2=true";

        var response = await GetAsync(query);

        return await DeserializeAsync<SearchResponse<T>>(response);
    }

    public async Task<HttpResponseMessage> GetAsync(string query)
    {
        await PingCheckAsync();

        var restUrl = $"{_session.LoginResponse!.RestUrl}{query}";

        return await _client.GetAsync(restUrl);
    }

    // This might be wrapped to ApiCreateEntity
    public async Task<HttpResponseMessage> ApiPutAsync(string query, HttpContent content)
    {
        await PingCheckAsync();

        var restUrl = $"{_session.LoginResponse!.RestUrl}{query}";

        return await _client.PutAsync(restUrl, content);
    }

    public async Task<Result<ChangeResponse>> PutAsJsonAsync(EntityType type, object content)
    {
        var response = await PutAsJsonAsync($"entity/{type}", content);

        return await GetChangeResponseAsync(response).ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> PutAsJsonAsync(string query, object content)
    {
        await PingCheckAsync();

        var restUrl = $"{_session.LoginResponse!.RestUrl}{query}";

        try
        {
            return await _client.PutAsJsonAsync(restUrl, content);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PutAsJsonAsync");
        }

        return new HttpResponseMessage();
    }

    // Probably this pattern should be used across
    public async Task<Result<ChangeResponse>> PostAsJsonAsync(EntityType type, int entityId, object content)
    {
        var response = await PostAsJsonAsync($"entity/{type}/{entityId}", content);

        return await GetChangeResponseAsync(response).ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync(string query, object content)
    {
        await PingCheckAsync();

        var restUrl = $"{_session.LoginResponse!.RestUrl}{query}";

        try
        {
            return await _client.PostAsJsonAsync(restUrl, content);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PostAsJsonAsync");
        }

        return new HttpResponseMessage();
    }

    public async Task<HttpResponseMessage> PostAsync(string query, HttpContent? content)
    {
        await PingCheckAsync();

        var restUrl = $"{_session.LoginResponse!.RestUrl}{query}";

        try
        {
            return await _client.PostAsync(restUrl, content);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PostAsync");
        }

        return new HttpResponseMessage();
    }

    public async Task<HttpResponseMessage> ApiDeleteAsync(string query)
    {
        await PingCheckAsync();

        var restUrl = $"{_session.LoginResponse!.RestUrl}{query}";

        return await _client.DeleteAsync(restUrl);
    }

    public async Task UpdateAsync<T>(int id, string entityName, T updateDto) => await PostAsync($"entity/{entityName}/{id}",
            new StringContent(JsonSerializer.Serialize(updateDto, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }), Encoding.UTF8, "application/json"));

    public async Task MassUpdateAsync<T>(string entityName, T updateDto) => await PostAsync($"massUpdate/{entityName}?",
            new StringContent(JsonSerializer.Serialize(updateDto, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Always
            }), Encoding.UTF8, "application/json"));

    public async Task DeleteAsync(int id, string entityName) => await ApiDeleteAsync($"entity/{entityName}/{id}?");

    //ToDo
    //public List<T> MapResults<T>(IEnumerable<JObject> data)
    //{
    //    var objects = data.Select(s => s.ToObject<T>()).ToList();

    //    return objects;
    //}

    public static string GetQuotedString(IEnumerable<string> list) => string.Join(" OR ", list.Select(s => $"\"{s}\""));

    public Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
        => response.DeserializeAsync<T>(_logger);

    public async Task<T?> EntityAsync<T>(string query)
    {
        query = $"entity/{query}";

        var response = await GetAsync(query);

        var entityResponse = await DeserializeAsync<EntityResponse<T>>(response);

        if (entityResponse is null)
        {
            _logger.LogError("EntityAsync, Entity deserialization failed.");

            return default;
        }

        return entityResponse.Data;
    }

    /// <summary>
    /// Performs an asynchronous search query with pagination, returning a list of results.
    /// Supports a maximum result count through the <paramref name="total"/> parameter.
    /// </summary>
    /// <typeparam name="T">The type of items in the search result.</typeparam>
    /// <param name="searchTerm">The search term or query string.</param>
    /// <param name="queryCount">The number of results to fetch per request (defaults to a constant QueryCount).</param>
    /// <param name="total">The maximum number of results to return (optional).</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of results.</returns>
    public async Task<List<T>> SearchAsync<T>(string searchTerm, int queryCount = QueryCount, int total = 0)
    {
        var result = await SearchApiAsync<T>(searchTerm, queryCount);

        if (result == null || result.Data == null) return [];

        var data = new List<T>(result.Data);

        var fetchedCount = result.Count;

        // If total is provided and we've already fetched enough data, return
        if (total != 0 && fetchedCount >= total)
        {
            return data;
        }

        // Fetch more data if necessary
        for (var i = fetchedCount; i < result.Total; i += result.Count)
        {
            result = await SearchApiAsync<T>(searchTerm, queryCount, start: i);

            if (result == null || result.Data == null)
            {
                break;
            }

            data.AddRange(result.Data);

            if (total != 0 && total <= i) break;
        }

        return data ?? [];
    }

    public void LogWarning(string text) => _logger.LogWarning(text);

    private async Task PingCheckAsync()
    {
        _logger.LogDebug("Next token refresh at {expiryDate}", _session.Ping.SessionExpiryDate);

        if (!_session.IsValid)
        {
            _logger.LogError("{0}, Not logged in yet.", nameof(PingCheckAsync));
        }

        if (_session.Ping?.Valid ?? false) return;

        _apiCallCounter++;

        try
        {
            using var response = await _client.GetAsync($"{_session.LoginResponse!.RestUrl}/ping");

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

            await _session.ConnectAsync();
        }

        if (_session.Ping is null)
        {
            _logger.LogError("PingCheckAsync, Ping is null.");

            return;
        }

        _logger.LogDebug("Next token refresh at {0}", _session.Ping.SessionExpiryDate);

        if (_session.Ping.Valid) return;

        _logger.LogInformation($"Token refresh on {_apiCallCounter} API call.");

        await _session.RefreshTokenAsync();
    }

    private static async Task<Result<ChangeResponse>> GetChangeResponseAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<ChangeResponse>(response.ReasonPhrase);
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

    ///// <summary>
    ///// Gives only total, only for search/queries
    ///// </summary>
    ///// <param name="query"></param>
    ///// <returns>total number</returns>
    //public async Task<int> TotalAsync(string query)
    //{
    //    var response = await ApiSearchAsync(query, 1);

    //    return response.Total;
    //}

    //ToDo
    //public async Task<SearchResponse<JObject>> ApiSearchAsync(string query, int count, int start = 0) => await ApiSearchAsync<JObject>(query, count, start);

    //[Obsolete("Investigate if this should be removed", true)]
    //public async Task<QueryResponse> ApiQueryAsync(string query, int count, int start = 0)
    //{
    //    query = $"query/{query}&start={start}&count={count}&showTotalMatched=true&usev2=true";

    //    var response = await ApiGetAsync(query);

    //    return await DeserializeAsync<QueryResponse>(response);
    //}

    //public async Task<DynamicQueryResponse> ApiQueryToDynamicAsync(string query, int count, int start = 0)
    //{
    //    query = $"query/{query}&start={start}&count={count}&showTotalMatched=true&usev2=true";

    //    var response = await ApiGetAsync(query);

    //    ApiCallCounter();

    //    return await response.Content.ReadAsAsync<DynamicQueryResponse>();
    //}

    //[Obsolete("Investigate if this should be removed", true)]
    //private static void LogList(List<JObject> list)
    //{
    //    foreach (var item in list)
    //    {
    //        Logger(JsonConvert.SerializeObject(item) + "\n");
    //    }
    //}

    //public string GetBullhornRestToken() => _authorization.LoginResponse?.BhRestToken ?? string.Empty;

    //[Obsolete("Investigate if this should be removed", true)]
    //private static void Logger(string content) => File.AppendAllText("temp-logfile.txt", content);

    //private string GetToken() => $"BhRestToken={_authorization.LoginResponse.BhRestToken}";

    //private void UpdateBhRestTokenHeader()
    //{
    //    _httpClient.DefaultRequestHeaders.Remove("BhRestToken");
    //    _httpClient.DefaultRequestHeaders.Add("BhRestToken", _authorization.LoginResponse.BhRestToken);
    //}
}

// Other query examples
//search/Note?fields=id,dateAdded,action,commentingPerson&query=dateAdded:[20210101000000 TO *] AND action:'Phone Call'&sort=-dateAdded
//[ContestType.GdprWithDrawn] = "Candidate?fields=id&query=notes.id:\"^^action:(\\\"gdpr withdrawn\\\") AND isDeleted:false\""
//            var query = "Candidate?fields=id,status,firstName,dateAdded,owner,email,email2,email3,phone,phone2,phone3,mobile,workPhone,placements[0](id),sendouts(id,dateAdded),fileAttachments(id)&query=isDeleted:0 AND -status:\"Archive\" AND -email:[\"\" TO *] AND -email2:[\"\" TO *] AND -email3:[\"\" TO *] AND -mobile:[\"\" TO *] AND -phone:[\"\" TO *] AND -placements.id:[0 TO 99999999999] AND -interviews.id:[0 TO 99999999999] AND -fileAttachments.id:[0 TO 99999999999] AND -notes.id:\"^^action:(\\\"Parse Failed Remove\\\") AND isDeleted:false\" AND -notes.id:\"^^action:(\\\"Parse Failed Keep\\\") AND isDeleted:false\" &sort=-dateAdded";
//            var query = "Candidate?fields=id,status,email,email2,email3,mobile,phone,notes(id,action)&query=isDeleted:0 AND notes.id:\"^^action:(\\\"No Files Remove\\\") AND isDeleted:false\" AND (email:[\"\" TO *] OR email2:[\"\" TO *] OR email3:[\"\" TO *] OR mobile:[\"\" TO *]  OR phone:[\"\" TO *] ) AND -phone:\"44 01700000000\"