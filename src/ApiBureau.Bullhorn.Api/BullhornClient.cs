namespace ApiBureau.Bullhorn.Api;

public class BullhornClient
{
    private readonly ApiConnection _client;

    public AppointmentEndpoint Appointment { get; }
    public CandidateEndpoint Candidate { get; }
    public ContryEndpoint Country { get; }
    public DepartmentEndpoint Department { get; }
    public JobOrderEndpoint JobOrder { get; }
    public JobSubmissionEndpoint JobSubmission { get; }
    public PlacementEndpoint Placement { get; }
    public MassUpdateEndpoint MassUpdate { get; }

    public BullhornClient(ApiConnection client)
    {
        Appointment = new(client);
        Candidate = new(client);
        Country = new(client);
        Department = new(client);
        JobOrder = new(client);
        JobSubmission = new(client);
        Placement = new(client);
        MassUpdate = new(client);
        _client = client;
    }

    // ToDo Refactor this so the check connection is done automatically
    public Task CheckConnectionAsync() => _client.CheckConnectionAsync();

    //internal Task<List<T>> QueryAsync<T>(string query) => throw new NotImplementedException();
}