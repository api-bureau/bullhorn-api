namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class EventEndpoint
{
    private readonly BullhornHttpClient _client;

    internal EventEndpoint(BullhornHttpClient client, string requestUrl)
    {
        _client = client;
        RequestUrl = requestUrl;
    }

    private string RequestUrl { get; }

    /// <summary>
    /// Lets you subscribe to Bullhorn event types
    /// </summary>
    /// <param name="subscriptionId">Used to retrieve changes, remember this id.</param>
    /// <param name="entityNames">Comma separated entities e.g. "Candidate,ClientContact"</param>
    /// <param name="eventTypes">Comma separated events e.g. "inserted,updated,deleted"</param>
    /// <returns></returns>
    public async Task<EventSubscribeDto?> SubscribeAsync(string subscriptionId, string entityNames, string eventTypes, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(subscriptionId)) throw new ArgumentException(null, nameof(subscriptionId));

        if (string.IsNullOrWhiteSpace(entityNames)) throw new ArgumentException(null, nameof(entityNames));

        if (string.IsNullOrWhiteSpace(eventTypes)) throw new ArgumentException(null, nameof(eventTypes));

        var query = $"{RequestUrl}/subscription/{subscriptionId}?type=entity&names={entityNames}&eventTypes={eventTypes}";

        var response = await _client.ApiPutAsync(query, new StringContent(string.Empty), cancellationToken);

        return await response.DeserializeAsync<EventSubscribeDto>();
    }

    public async Task<EventSubscribeDto?> ReSubscribeAsync(string subscriptionId, string entityNames, string eventTypes, CancellationToken cancellationToken)
    {
        var unsubscribed = await UnSubscribeAsync(subscriptionId, cancellationToken);

        if (unsubscribed != null && unsubscribed.Result) return await SubscribeAsync(subscriptionId, entityNames, eventTypes, cancellationToken);

        return null;
    }

    public async Task<EventUnSubscribeDto?> UnSubscribeAsync(string subscriptionId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(subscriptionId)) throw new ArgumentException(null, nameof(subscriptionId));

        var query = $"{RequestUrl}/subscription/{subscriptionId}";

        var response = await _client.ApiDeleteAsync(query, cancellationToken);

        return await response.DeserializeAsync<EventUnSubscribeDto>();
    }

    /// <summary>
    /// Use try catch when calling this method. The subscription returns empty string if no data which is not standard and so it crashes.
    /// </summary>
    /// <param name="subscriptionId"></param>
    /// <returns></returns>
    public async Task<EventsDto?> GetAsync(string subscriptionId, CancellationToken cancellationToken)
    {
        var query = $"{RequestUrl}/subscription/{subscriptionId}?maxEvents=100";

        _client.LogWarning("Placement Event Subscription: This call might throw an error if expected the input to start with a no valid JSON cancellationToken (empty string), meaning no data in this case.");

        var response = await _client.GetAsync(query, cancellationToken);

        return await response.DeserializeAsync<EventsDto>();
    }
}