namespace ApiBureau.Bullhorn.Api;

public class BullhornClient
{
    public AppointmentEndpoint Appointment { get; }
    public ContryEndpoint Country { get; }
    public JobOrderEndpoint JobOrder { get; }
    public JobSubmissionEndpoint JobSubmission { get; }
    public PlacementEndpoint Placement { get; }
    public MassUpdateEndpoint MassUpdate { get; }

    public BullhornClient(ApiConnection client)
    {
        Appointment = new(client);
        Country = new(client);
        JobOrder = new(client);
        JobSubmission = new(client);
        Placement = new(client);
        MassUpdate = new(client);
    }

    internal Task<List<T>> QueryAsync<T>(string query) => throw new NotImplementedException();
}