using ApiBureau.Bullhorn.Api.Endpoints;
using ApiBureau.Bullhorn.Api.Interfaces;

namespace ApiBureau.Bullhorn.Api;

public sealed class BullhornClient : IBullhornClient
{
    private readonly BullhornHttpClient _httpClient;

    public BullhornAdvancedClient Advanced { get; }
    public AppointmentEndpoint Appointments { get; }
    public CandidateEndpoint Candidates { get; }
    public CandidateWorkHistoryEndpoint CandidateWorkHistories { get; }
    public ClientContactEndpoint ClientContacts { get; }
    public ClientCorporationEndpoint ClientCorporations { get; }
    public CorporateUserEndpoint CorporateUsers { get; }
    public CorporationDepartmentEndpoint CorporationDepartments { get; }
    public CountryEndpoint Countries { get; }
    public DepartmentEndpoint Departments { get; }
    public EntityEditHistoryEndpoint EntityEditHistories { get; }
    public EventEndpoint Events { get; }
    public FileAttachmentEndpoint FileAttachments { get; }
    public FileEndpoint Files { get; }
    public JobOrderEndpoint JobOrders { get; }
    public JobSubmissionEndpoint JobSubmissions { get; }
    public JobSubmissionHistoryEndpoint JobSubmissionHistories { get; }
    public MassUpdateEndpoint MassUpdates { get; }
    public NoteEndpoint Notes { get; }
    public OpportunityEndpoint Opportunities { get; }
    public PlacementChangeRequestEndpoint PlacementChangeRequests { get; }
    public PlacementCommissionEndpoint PlacementCommissions { get; }
    public PlacementEndpoint Placements { get; }
    public ResumeEndpoint Resumes { get; }
    public SendoutEndpoint Sendouts { get; }

    public BullhornClient(BullhornHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        Advanced = new(_httpClient);
        Appointments = new(_httpClient, "Appointment");
        Candidates = new(_httpClient, "Candidate");
        CandidateWorkHistories = new(_httpClient, "CandidateWorkHistory");
        ClientContacts = new(_httpClient, "ClientContact");
        ClientCorporations = new(_httpClient, "ClientCorporation");
        CorporateUsers = new(_httpClient, "CorporateUser");
        CorporationDepartments = new(_httpClient, "CorporationDepartment");
        Countries = new(_httpClient, "Country");
        Departments = new(_httpClient, "Department");
        EntityEditHistories = new(_httpClient);
        Events = new(_httpClient, "event");
        FileAttachments = new(_httpClient);
        Files = new(_httpClient, "file");
        JobOrders = new(_httpClient, "JobOrder");
        JobSubmissions = new(_httpClient, "JobSubmission");
        JobSubmissionHistories = new(_httpClient, "JobSubmissionHistory");
        MassUpdates = new(_httpClient, "massUpdate");
        Notes = new(_httpClient, "Note");
        Opportunities = new(_httpClient, "Opportunity");
        Placements = new(_httpClient, "Placement");
        PlacementChangeRequests = new(_httpClient, "PlacementChangeRequest");
        PlacementCommissions = new(_httpClient, "PlacementCommission");
        Resumes = new(_httpClient, "resume");
        Sendouts = new(_httpClient, "Sendout");
    }

    // ToDo Refactor this so the check connection is done automatically
    public Task<bool> CheckConnectionAsync(IProgress<string>? progress = null) => _httpClient.CheckConnectionAsync(progress);

}