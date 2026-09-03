namespace ApiBureau.Bullhorn.Api.Internals;

internal sealed class SearchOperations<T>(BullhornHttpClient client, string resourcePath, string defaultFields)
    : EntityOperations<T>(client, resourcePath, defaultFields)
{
    internal Task<List<T>> SearchFromAsync(DateTime dateTimeFrom, string? fields = null, CancellationToken token = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO *]", token: token);

    internal Task<List<T>> SearchFromToAsync(DateTime dateTimeFrom, DateTime dateTimeTo, string? fields = null, CancellationToken token = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO {dateTimeTo:yyyyMMddHHmmss}]", token: token);

    internal Task<List<T>> SearchNewAndUpdatedFromAsync(DateTime dateTimeFrom, string? fields = null, CancellationToken token = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO *] OR dateLastModified:[{dateTimeFrom:yyyyMMddHHmmss} TO *]", token: token);

    internal Task<List<T>> GetUpdatedFromAsync(DateTime dateTime, string? fields = null, CancellationToken token = default)
        => ExecuteAsync($"{ResourcePath}?fields={fields ?? DefaultFields}&query=dateLastModified:[{dateTime:yyyyMMddHHmmss} TO *]", token: token);

    internal async Task<List<T>> ExecuteAsync(string searchTerm, int queryCount = BullhornHttpClient.QueryCount, int total = 0, CancellationToken token = default)
    {
        var result = await Client.SearchPageAsync<T>(searchTerm, queryCount, token: token).ConfigureAwait(false);

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
            result = await Client.SearchPageAsync<T>(searchTerm, queryCount, i, token).ConfigureAwait(false);

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