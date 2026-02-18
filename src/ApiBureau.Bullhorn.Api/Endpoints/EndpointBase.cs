namespace ApiBureau.Bullhorn.Api.Endpoints;

public abstract class EndpointBase(ApiConnection apiConnection, string requestUrl)
{
    protected ApiConnection ApiConnection { get; } = apiConnection;
    public string RequestUrl { get; } = requestUrl;
}