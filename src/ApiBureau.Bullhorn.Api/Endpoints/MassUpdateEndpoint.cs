namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// Provides access to Bullhorn mass update endpoints.
/// </summary>
public class MassUpdateEndpoint : EndpointBase
{
    public MassUpdateEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl) { }

    /// <summary>
    /// Retrieves the entity types that support mass update operations.
    /// </summary>
    /// <param name="token">The cancellation token used to cancel the request.</param>
    /// <returns>The HTTP response containing the supported entity types.</returns>
    public Task<HttpResponseMessage> GetAsync(CancellationToken token) => ApiConnection.GetAsync($"/{RequestUrl}", token);

    /// <summary>
    /// Retrieves the properties that can be mass updated for the specified entity type.
    /// </summary>
    /// <param name="type">The Bullhorn entity type to inspect.</param>
    /// <param name="token">The cancellation token used to cancel the request.</param>
    /// <returns>The HTTP response containing the mass-updateable properties for the entity type.</returns>
    public Task<HttpResponseMessage> GetAsync(EntityType type, CancellationToken token)
        => ApiConnection.GetAsync($"/{RequestUrl}/{type}", token);

    /// <summary>
    /// Performs a mass update for the specified entity type using the identifiers and field values supplied in the request body.
    /// </summary>
    /// <param name="type">The Bullhorn entity type to update.</param>
    /// <param name="data">The payload containing the target entity identifiers and values to apply.</param>
    /// <param name="token">The cancellation token used to cancel the request.</param>
    /// <returns>The HTTP response returned by the Bullhorn mass update endpoint.</returns>
    public Task<HttpResponseMessage> UpdateAsync(EntityType type, object data, CancellationToken token)
        => ApiConnection.PostAsJsonAsync($"/{RequestUrl}/{type}", data, token);
}