namespace ApiBureau.Bullhorn.Api.Internals;

internal sealed class SearchOperations<T> : EntityOperations<T>
{
    internal SearchOperations(BullhornHttpClient client, string resourcePath, string defaultFields)
        : base(client, resourcePath, defaultFields)
    {
    }

    internal Task<List<T>> GetAddedSinceAsync(DateTime dateTimeFrom, string? fields = null, CancellationToken cancellationToken = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO *]", cancellationToken: cancellationToken);

    internal Task<List<T>> GetAddedBetweenAsync(DateTime dateTimeFrom, DateTime dateTimeTo, string? fields = null, CancellationToken cancellationToken = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO {dateTimeTo:yyyyMMddHHmmss}]", cancellationToken: cancellationToken);

    internal Task<List<T>> GetChangedSinceAsync(DateTime dateTimeFrom, string? fields = null, CancellationToken cancellationToken = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO *] OR dateLastModified:[{dateTimeFrom:yyyyMMddHHmmss} TO *]", cancellationToken: cancellationToken);

    internal Task<List<T>> GetUpdatedSinceAsync(DateTime dateTime, string? fields = null, CancellationToken cancellationToken = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&query=dateLastModified:[{dateTime:yyyyMMddHHmmss} TO *]", cancellationToken: cancellationToken);

    internal async Task<List<T>> ExecuteAsync(string searchTerm, int queryCount = BullhornHttpClient.QueryCount, int total = 0, CancellationToken cancellationToken = default)
    {
        var result = await Client.SearchPageAsync<T>(searchTerm, queryCount, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result?.Data is null)
        {
            return [];
        }

        var data = new List<T>(result.Data);
        var fetchedCount = result.Count;

        if (total != 0 && fetchedCount >= total)
        {
            return data;
        }

        for (var i = fetchedCount; i < result.Total; i += result.Count)
        {
            result = await Client.SearchPageAsync<T>(searchTerm, queryCount, i, cancellationToken).ConfigureAwait(false);

            if (result?.Data is null)
            {
                break;
            }

            data.AddRange(result.Data);

            if (total != 0 && total <= i)
            {
                break;
            }
        }

        return data;
    }
}