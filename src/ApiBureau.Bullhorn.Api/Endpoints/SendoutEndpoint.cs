namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class SendoutEndpoint : QueryEndpointBase<SendoutDto>
{
    private const string EntityDefaultFields = "id,candidate,user,dateAdded,jobOrder,clientContact,clientCorporation";

    internal SendoutEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

}