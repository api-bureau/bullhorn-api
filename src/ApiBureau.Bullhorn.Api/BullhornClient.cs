namespace ApiBureau.Bullhorn.Api;

public class BullhornClient
{
    public AppointmentEndpoint Appointment { get; }
    public JobOrderEndpoint JobOrder { get; }
    public PlacementEndpoint Placement { get; }
    public MassUpdateEndpoint MassUpdate { get; }

    public BullhornClient(ApiConnection client)
    {
        Appointment = new(client);
        JobOrder = new(client);
        Placement = new(client);
        MassUpdate = new(client);
    }

    internal Task<List<T>> QueryAsync<T>(string query) => throw new NotImplementedException();
}