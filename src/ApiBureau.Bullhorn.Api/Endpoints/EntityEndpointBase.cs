namespace ApiBureau.Bullhorn.Api.Endpoints;

public abstract class EntityEndpointBase<T>(ApiConnection apiConnection, string requestUrl, string defaultFields)
    : EndpointBase(apiConnection, requestUrl)
{
    public string DefaultFields { get; } = defaultFields;

    /// <summary>
    /// Retrieves a single entity by its ID using the <c>/entity</c> endpoint.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <param name="fields">
    /// Comma-separated list of fields to return.
    /// Defaults to <see cref="DefaultFields"/> when <see langword="null"/>.
    /// </param>
    /// <param name="token">A token to cancel the asynchronous operation.</param>
    /// <returns>The matching entity, or <see langword="null"/> if not found.</returns>
    public async Task<T?> GetAsync(int id, string? fields = null, CancellationToken token = default)
    {
        var query = $"{RequestUrl}/{id}?fields={fields ?? DefaultFields}";

        return await ApiConnection.EntityAsync<T>(query, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves one or more entities by their IDs using the <c>/entity</c> endpoint.
    /// When a single ID is provided, delegates to <see cref="GetAsync(int, string?, CancellationToken)"/>.
    /// </summary>
    /// <param name="ids">The unique identifiers of the entities to retrieve.</param>
    /// <param name="fields">
    /// Comma-separated list of fields to return.
    /// Defaults to <see cref="DefaultFields"/> when <see langword="null"/>.
    /// </param>
    /// <param name="token">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of matching entities.</returns>
    public async Task<List<T>> GetAsync(IEnumerable<int> ids, string? fields = null, CancellationToken token = default)
    {
        var idList = ids.ToList();

        if (idList is [var id])
        {
            var entity = await GetAsync(id, fields, token).ConfigureAwait(false);

            return entity is null ? [] : [entity];
        }

        var query = $"{RequestUrl}/{string.Join(",", idList)}?fields={fields ?? DefaultFields}";

        return await ApiConnection.EntityAsync<List<T>>(query, token).ConfigureAwait(false) ?? [];
    }
}