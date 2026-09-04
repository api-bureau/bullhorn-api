namespace ApiBureau.Bullhorn.Api.Internals;

internal sealed class QueryOperations<T> : EntityOperations<T>
{
    private const string DefaultWhere = "id>0";

    internal QueryOperations(BullhornHttpClient client, string resourcePath, string defaultFields)
        : base(client, resourcePath, defaultFields)
    {
    }

    internal Task<List<T>> GetAddedSinceAsync(long timestampFrom, string? fields = null, CancellationToken cancellationToken = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&where=dateAdded>={timestampFrom}", cancellationToken);

    internal Task<List<T>> GetAddedBetweenAsync(long timestampFrom, long timestampTo, string? fields = null, CancellationToken cancellationToken = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&where=dateAdded>={timestampFrom} AND dateAdded<{timestampTo}", cancellationToken);

    internal Task<List<T>> GetChangedSinceAsync(long timestampFrom, string? fields = null, CancellationToken cancellationToken = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&where=dateAdded>={timestampFrom} OR dateLastModified>{timestampFrom}", cancellationToken);

    internal Task<List<T>> GetUpdatedSinceAsync(long timestampFrom, string? fields = null, CancellationToken cancellationToken = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&where=dateLastModified>{timestampFrom}", cancellationToken);

    internal Task<List<T>> GetWhereAsync(string? fields = null, string? defaultWhere = DefaultWhere, CancellationToken cancellationToken = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&where={defaultWhere}", cancellationToken);

    internal async Task<List<T>> ExecuteAsync(string query, CancellationToken cancellationToken)
    {
        var items = new List<T>();
        var result = await Client.QueryPageAsync<T>(query, BullhornHttpClient.QueryCount, cancellationToken: cancellationToken).ConfigureAwait(false);

        items.AddRange(result?.Data ?? []);

        if (result is null)
        {
            return items;
        }

        for (var start = result.Count; start < result.Total;)
        {
            var page = await Client.QueryPageAsync<T>(query, BullhornHttpClient.QueryCount, start, cancellationToken).ConfigureAwait(false);

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