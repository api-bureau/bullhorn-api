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
            throw new HttpRequestException("Bullhorn search response did not contain data.");
        }

        var data = new List<T>(result.Data);
        var fetchedCount = result.Count;
        var expectedTotal = result.Total;

        if (total != 0 && fetchedCount >= total)
        {
            return data;
        }

        for (var i = fetchedCount; i < expectedTotal; i += result.Count)
        {
            if (result.Count <= 0)
                throw new HttpRequestException("Bullhorn search pagination stopped before all results were returned.");

            result = await Client.SearchPageAsync<T>(searchTerm, queryCount, i, cancellationToken).ConfigureAwait(false);

            if (result?.Data is null || result.Count <= 0 || result.Data.Count == 0)
            {
                throw new HttpRequestException("Bullhorn search page did not contain data.");
            }

            data.AddRange(result.Data);

            if (total != 0 && total <= data.Count)
            {
                break;
            }
        }

        return data;
    }
}