using ApiBureau.Bullhorn.Api.Endpoints;

namespace ApiBureau.Bullhorn.Api;

public class BullhornClient
{
    public PlacementEndpoint Placement { get; }
    public MassUpdateEndpoint MassUpdate { get; }

    public BullhornClient(ApiConnection client)
    {
        Placement = new(client);
        MassUpdate = new(client);
    }
}