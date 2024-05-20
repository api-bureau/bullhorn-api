using Microsoft.Extensions.Logging;

namespace ApiBureau.Bullhorn.Api;

public class BullhornClient : IBullhornClient //: BaseClient
{
    private readonly ApiConnection _apiConnection;

    public AppointmentEndpoint Appointment { get; }
    public CandidateEndpoint Candidate { get; }
    public CandidateWorkHistoryEndpoint CandidateWorkHistory { get; }
    public ClientContactEndpoint ClientContact { get; }
    public ClientCorporationEndpoint ClientCorporation { get; }
    public CorporateUserEndpoint CorporateUser { get; }
    public CorporationDepartmentEndpoint CorporationDepartment { get; }
    public CountryEndpoint Country { get; }
    public DepartmentEndpoint Department { get; }
    public EntityEditHistoryEndpoint EntityEditHistory { get; }
    public EventEndpoint Event { get; }
    public FileAttachmentEndpoint FileAttachment { get; }
    public FileEndpoint File { get; }
    public JobOrderEndpoint JobOrder { get; }
    public JobSubmissionEndpoint JobSubmission { get; }
    public JobSubmissionHistoryEndpoint JobSubmissionHistory { get; }
    public MassUpdateEndpoint MassUpdate { get; }
    public NoteEndpoint Note { get; }
    public OpportunityEndpoint Opportunity { get; }
    public PlacementChangeRequestEndpoint PlacementChangeRequest { get; }
    public PlacementCommissionEndpoint PlacementCommission { get; }
    public PlacementEndpoint Placement { get; }
    public ResumeEndpoint Resume { get; }
    public SendoutEndpoint Sendout { get; }

    // ToDo: Shall we move the below to the setter above?
    public BullhornClient(HttpClient client, IOptions<BullhornSettings> settings, ILogger<ApiConnection> logger)
    {
        _apiConnection = new ApiConnection(client, settings, logger);

        Appointment = new(_apiConnection, "Appointment");
        Candidate = new(_apiConnection, "Candidate");
        CandidateWorkHistory = new(_apiConnection, "CandidateWorkHistory");
        ClientContact = new(_apiConnection, "ClientContact");
        ClientCorporation = new(_apiConnection, "ClientCorporation");
        CorporateUser = new(_apiConnection, "CorporateUser");
        CorporationDepartment = new(_apiConnection, "CorporationDepartment");
        Country = new(_apiConnection, "Country");
        Department = new(_apiConnection, "Department");
        EntityEditHistory = new(_apiConnection);
        Event = new(_apiConnection, "event");
        FileAttachment = new(_apiConnection);
        File = new(_apiConnection, "file");
        JobOrder = new(_apiConnection, "JobOrder");
        JobSubmission = new(_apiConnection, "JobSubmission");
        JobSubmissionHistory = new(_apiConnection, "JobSubmissionHistory");
        MassUpdate = new(_apiConnection, "massUpdate");
        Note = new(_apiConnection, "Note");
        Opportunity = new(_apiConnection, "Opportunity");
        Placement = new(_apiConnection, "Placement");
        PlacementChangeRequest = new(_apiConnection, "PlacementChangeRequest");
        PlacementCommission = new(_apiConnection, "PlacementCommission");
        Resume = new(_apiConnection, "resume");
        Sendout = new(_apiConnection, "Sendout");
    }

    // ToDo Refactor this so the check connection is done automatically
    public Task CheckConnectionAsync() => _apiConnection.CheckConnectionAsync();

    public Task<List<T>> QueryAsync<T>(string query) => _apiConnection.QueryAsync<T>(query);

    // ToDo Temporary Location
    public Task<HttpResponseMessage> ApiGetAsync(string query, int count, int start = 0)
        => _apiConnection.ApiGetAsync(query, count, start);
}