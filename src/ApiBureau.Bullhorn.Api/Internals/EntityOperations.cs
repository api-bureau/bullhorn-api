namespace ApiBureau.Bullhorn.Api.Internals;

internal class EntityOperations<T>(BullhornHttpClient client, string resourcePath, string defaultFields)
{
    protected BullhornHttpClient Client { get; } = client;

    protected string ResourcePath { get; } = resourcePath;

    protected string DefaultFields { get; } = defaultFields;

    internal async Task<T?> GetAsync(int id, string? fields = null, CancellationToken token = default)
    {
        var query = $"entity/{ResourcePath}/{id}?fields={fields ?? DefaultFields}";
        var response = await Client.GetAsync(query, token).ConfigureAwait(false);
        var entityResponse = await Client.DeserializeAsync<EntityResponse<T>>(response).ConfigureAwait(false);

        if (entityResponse is null)
        {
            Client.LogError("Entity deserialization failed for {ResourcePath}.", ResourcePath);

            return default;
        }

        return entityResponse.Data;
    }

    internal async Task<List<T>> GetAsync(IEnumerable<int> ids, string? fields = null, CancellationToken token = default)
    {
        var idList = ids.ToList();

        if (idList is [var id])
        {
            var entity = await GetAsync(id, fields, token).ConfigureAwait(false);

            return entity is null ? [] : [entity];
        }

        var query = $"entity/{ResourcePath}/{string.Join(",", idList)}?fields={fields ?? DefaultFields}";
        var response = await Client.GetAsync(query, token).ConfigureAwait(false);
        var entityResponse = await Client.DeserializeAsync<EntityResponse<List<T>>>(response).ConfigureAwait(false);

        if (entityResponse is null)
        {
            Client.LogError("Entity deserialization failed for {ResourcePath}.", ResourcePath);
        }

        return entityResponse?.Data ?? [];
    }
}