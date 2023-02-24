using Microsoft.Extensions.Logging;

namespace ApiBureau.Bullhorn.Api;

public class BullhornClient //: BaseClient
{
    private readonly ApiConnection _apiConnection;

    public AppointmentEndpoint Appointment { get; }
    public CandidateEndpoint Candidate { get; }
    public CandidateWorkHistoryEndpoint CandidateWorkHistory { get; }
    public ClientContactEndpoint ClientContact { get; }
    public ClientCorporationEndpoint ClientCorporation { get; }
    public CorporationDepartmentEndpoint CorporationDepartment { get; }
    public CorporateUserEndpoint CorporateUser { get; }
    public CountryEndpoint Country { get; }
    public DepartmentEndpoint Department { get; }
    public EntityEditHistoryEndpoint EntityEditHistory { get; }
    public JobOrderEndpoint JobOrder { get; }
    public JobSubmissionEndpoint JobSubmission { get; }
    public JobSubmissionHistoryEndpoint JobSubmissionHistory { get; }
    public NoteEndpoint Note { get; }
    public PlacementEndpoint Placement { get; }
    public PlacementCommissionEndpoint PlacementCommission { get; }
    public PlacementChangeRequestEndpoint PlacementChangeRequest { get; }
    public ResumeEndpoint Resume { get; }
    public MassUpdateEndpoint MassUpdate { get; }
    public SendoutEndpoint Sendout { get; }

    public BullhornClient(HttpClient client, IOptions<BullhornSettings> settings, ILogger<ApiConnection> logger)
    {
        _apiConnection = new ApiConnection(client, settings, logger);

        Appointment = new(_apiConnection, "Appointment");
        Candidate = new(_apiConnection, "Candidate");
        CandidateWorkHistory = new(_apiConnection, "CandidateWorkHistory");
        ClientContact = new(_apiConnection, "ClientContact");
        ClientCorporation = new(_apiConnection, "ClientCorporation");
        CorporationDepartment = new(_apiConnection, "CorporationDepartment");
        CorporateUser = new(_apiConnection, "CorporateUser");
        Country = new(_apiConnection, "Country");
        Department = new(_apiConnection, "Department");
        EntityEditHistory = new(_apiConnection);
        JobOrder = new(_apiConnection, "JobOrder");
        JobSubmission = new(_apiConnection, "JobSubmission");
        JobSubmissionHistory = new(_apiConnection, "JobSubmissionHistory");
        Note = new(_apiConnection, "Note");
        Placement = new(_apiConnection, "Placement");
        PlacementCommission = new(_apiConnection, "PlacementCommission");
        PlacementChangeRequest = new(_apiConnection, "PlacementChangeRequest");
        Resume = new(_apiConnection, "resume");
        MassUpdate = new(_apiConnection, "massUpdate");
        Sendout = new(_apiConnection, "Sendout");
    }

    // ToDo Refactor this so the check connection is done automatically
    public Task CheckConnectionAsync() => _apiConnection.CheckConnectionAsync();

    public Task<List<T>> QueryAsync<T>(string query) => _apiConnection.QueryAsync<T>(query);
}