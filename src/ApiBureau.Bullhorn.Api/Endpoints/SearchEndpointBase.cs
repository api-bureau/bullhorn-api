namespace ApiBureau.Bullhorn.Api.Endpoints;

public abstract class SearchEndpointBase<T>(ApiConnection apiConnection, string requestUrl, string defaultFields)
    : EntityEndpointBase<T>(apiConnection, requestUrl, defaultFields)
{
    /// <summary>
    /// Searches for entities added on or after <paramref name="dateTimeFrom"/> using the <c>/search</c> endpoint,
    /// filtering by <c>dateAdded</c>.
    /// </summary>
    /// <param name="dateTimeFrom">The inclusive lower bound of the date range.</param>
    /// <param name="fields">
    /// Comma-separated list of fields to return.
    /// Defaults to <see cref="EntityEndpointBase{T}.DefaultFields"/> when <see langword="null"/>.
    /// </param>
    /// <param name="token">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of entities matching the search criteria.</returns>
    public async Task<List<T>> SearchFromAsync(DateTime dateTimeFrom, string? fields = null, CancellationToken token = default)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO *]";

        return await ApiConnection.SearchAsync<T>(query, token: token).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches for entities added within the specified date range using the <c>/search</c> endpoint,
    /// filtering by <c>dateAdded</c>.
    /// </summary>
    /// <param name="dateTimeFrom">The inclusive lower bound of the date range.</param>
    /// <param name="dateTimeTo">The inclusive upper bound of the date range.</param>
    /// <param name="fields">
    /// Comma-separated list of fields to return.
    /// Defaults to <see cref="EntityEndpointBase{T}.DefaultFields"/> when <see langword="null"/>.
    /// </param>
    /// <param name="token">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of entities matching the search criteria.</returns>
    public async Task<List<T>> SearchFromToAsync(DateTime dateTimeFrom, DateTime dateTimeTo, string? fields = null, CancellationToken token = default)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO {dateTimeTo:yyyyMMddHHmmss}]";

        return await ApiConnection.SearchAsync<T>(query, token: token).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches for entities that were added or last modified on or after <paramref name="dateTimeFrom"/>
    /// using the <c>/search</c> endpoint, filtering by <c>dateAdded</c> and <c>dateLastModified</c>.
    /// </summary>
    /// <param name="dateTimeFrom">The inclusive lower bound of the date range.</param>
    /// <param name="fields">
    /// Comma-separated list of fields to return.
    /// Defaults to <see cref="EntityEndpointBase{T}.DefaultFields"/> when <see langword="null"/>.
    /// </param>
    /// <param name="token">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of entities matching the search criteria.</returns>
    public async Task<List<T>> SearchNewAndUpdatedFromAsync(DateTime dateTimeFrom, string? fields = null, CancellationToken token = default)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&query=dateAdded:[{dateTimeFrom:yyyyMMddHHmmss} TO *] OR dateLastModified:[{dateTimeFrom:yyyyMMddHHmmss} TO *]";

        return await ApiConnection.SearchAsync<T>(query, token: token).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches for entities last modified on or after <paramref name="dateTime"/> using the <c>/search</c>
    /// endpoint, filtering by <c>dateLastModified</c>.
    /// </summary>
    /// <param name="dateTime">The inclusive lower bound of the date range.</param>
    /// <param name="fields">
    /// Comma-separated list of fields to return.
    /// Defaults to <see cref="EntityEndpointBase{T}.DefaultFields"/> when <see langword="null"/>.
    /// </param>
    /// <param name="token">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of entities matching the search criteria.</returns>
    public async Task<List<T>> GetUpdatedFromAsync(DateTime dateTime, string? fields = null, CancellationToken token = default)
    {
        var query = $"{RequestUrl}?fields={fields ?? DefaultFields}&query=dateLastModified:[{dateTime:yyyyMMddHHmmss} TO *]";

        return await ApiConnection.SearchAsync<T>(query, token: token).ConfigureAwait(false);
    }
}