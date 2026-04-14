namespace ApiBureau.Bullhorn.Api.Endpoints;

public class MassUpdateEndpoint : EndpointBase
{
    public MassUpdateEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl) { }

    /// <summary>
    /// Returns the list of entity types that support mass update.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<HttpResponseMessage> GetAsync(CancellationToken token) => ApiConnection.GetAsync($"/{RequestUrl}", token);

    /// <summary>
    /// Returns the list of entity properties for which mass update is supported on the specified entity type. Also
    /// returns the entitlement required for updating each property.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<HttpResponseMessage> GetAsync(EntityType type, CancellationToken token)
        => ApiConnection.GetAsync($"/{RequestUrl}/{type}", token);

    /// <summary>
    /// Performs a massUpdate on all entity records of the specified type for which the entity id is included in the
    /// request body.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="data"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<HttpResponseMessage> UpdateAsync(EntityType type, object data, CancellationToken token)
        => ApiConnection.PostAsJsonAsync($"/{RequestUrl}/{type}", data, token);
}