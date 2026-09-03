namespace ApiBureau.Bullhorn.Api.Internals;

internal sealed class QueryOperations<T>(BullhornHttpClient client, string resourcePath, string defaultFields)
    : EntityOperations<T>(client, resourcePath, defaultFields)
{
    private const string DefaultWhere = "id>0";

    internal Task<List<T>> QueryFromAsync(long timestampFrom, string? fields = null, CancellationToken token = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&where=dateAdded>={timestampFrom}", token);

    internal Task<List<T>> QueryFromToAsync(long timestampFrom, long timestampTo, string? fields = null, CancellationToken token = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&where=dateAdded>={timestampFrom} AND dateAdded<{timestampTo}", token);

    internal Task<List<T>> QueryNewAndUpdatedFromAsync(long timestampFrom, string? fields = null, CancellationToken token = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&where=dateAdded>={timestampFrom} OR dateLastModified>{timestampFrom}", token);

    internal Task<List<T>> QueryUpdatedFromAsync(long timestampFrom, string? fields = null, CancellationToken token = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&where=dateLastModified>{timestampFrom}", token);

    internal Task<List<T>> QueryWhereAsync(string? fields = null, string? defaultWhere = DefaultWhere, CancellationToken token = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&where={defaultWhere}", token);

    internal async Task<List<T>> ExecuteAsync(string query, CancellationToken token)
    {
        var items = new List<T>();
        var result = await Client.QueryPageAsync<T>(query, BullhornHttpClient.QueryCount, token: token).ConfigureAwait(false);

        items.AddRange(result?.Data ?? []);

        if (result is null)
        {
            return items;
        }

        for (var start = result.Count; start < result.Total;)
        {
            var page = await Client.QueryPageAsync<T>(query, BullhornHttpClient.QueryCount, start, token).ConfigureAwait(false);

            if (page is null || page.Count == 0)
            {
                break;
            }

            items.AddRange(page.Data ?? []);
            start += page.Count;
        }

        return items;
    }
}