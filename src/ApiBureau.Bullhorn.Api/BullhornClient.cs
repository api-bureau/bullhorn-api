using Microsoft.Extensions.Logging;

namespace ApiBureau.Bullhorn.Api;

public class BullhornClient
{
    private readonly ApiConnection _apiConnection;

    public AppointmentEndpoint Appointment { get; }
    public CandidateEndpoint Candidate { get; }
    public ClientContactEndpoint ClientContact { get; }
    public ClientCorporationEndpoint ClientCorporation { get; }
    public CountryEndpoint Country { get; }
    public DepartmentEndpoint Department { get; }
    public JobOrderEndpoint JobOrder { get; }
    public JobSubmissionEndpoint JobSubmission { get; }
    public NoteEndpoint Note { get; }
    public PlacementEndpoint Placement { get; }
    public PlacementChangeRequestEndpoint PlacementChangeRequest { get; }
    public MassUpdateEndpoint MassUpdate { get; }

    public BullhornClient(HttpClient client, IOptions<BullhornSettings> settings, ILogger<ApiConnection> logger)
    {
        _apiConnection = new ApiConnection(client, settings, logger);

        Appointment = new(_apiConnection);
        Candidate = new(_apiConnection);
        ClientContact = new(_apiConnection);
        ClientCorporation = new(_apiConnection);
        Country = new(_apiConnection);
        Department = new(_apiConnection);
        JobOrder = new(_apiConnection);
        JobSubmission = new(_apiConnection);
        Note = new(_apiConnection);
        Placement = new(_apiConnection);
        PlacementChangeRequest = new(_apiConnection);
        MassUpdate = new(_apiConnection);
    }

    // ToDo Refactor this so the check connection is done automatically
    public Task CheckConnectionAsync() => _apiConnection.CheckConnectionAsync();
}