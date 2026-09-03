using ApiBureau.Bullhorn.Api.Endpoints;
using ApiBureau.Bullhorn.Api.Interfaces;
using ApiBureau.Bullhorn.Api.Internals;

namespace ApiBureau.Bullhorn.Api;

public sealed class BullhornClient : IBullhornClient
{
    private readonly BullhornHttpClient _httpClient;

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

    public BullhornClient(BullhornHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        Appointment = new(_httpClient, "Appointment");
        Candidate = new(_httpClient, "Candidate");
        CandidateWorkHistory = new(_httpClient, "CandidateWorkHistory");
        ClientContact = new(_httpClient, "ClientContact");
        ClientCorporation = new(_httpClient, "ClientCorporation");
        CorporateUser = new(_httpClient, "CorporateUser");
        CorporationDepartment = new(_httpClient, "CorporationDepartment");
        Country = new(_httpClient, "Country");
        Department = new(_httpClient, "Department");
        EntityEditHistory = new(_httpClient);
        Event = new(_httpClient, "event");
        FileAttachment = new(_httpClient);
        File = new(_httpClient, "file");
        JobOrder = new(_httpClient, "JobOrder");
        JobSubmission = new(_httpClient, "JobSubmission");
        JobSubmissionHistory = new(_httpClient, "JobSubmissionHistory");
        MassUpdate = new(_httpClient, "massUpdate");
        Note = new(_httpClient, "Note");
        Opportunity = new(_httpClient, "Opportunity");
        Placement = new(_httpClient, "Placement");
        PlacementChangeRequest = new(_httpClient, "PlacementChangeRequest");
        PlacementCommission = new(_httpClient, "PlacementCommission");
        Resume = new(_httpClient, "resume");
        Sendout = new(_httpClient, "Sendout");
    }

    // ToDo Refactor this so the check connection is done automatically
    public Task<bool> CheckConnectionAsync(IProgress<string>? progress = null) => _httpClient.CheckConnectionAsync(progress);

    // ToDo move raw query access to client.Advanced during public API normalization.
    public Task<List<T>> QueryAsync<T>(string query, CancellationToken token)
        => new QueryOperations<T>(_httpClient, string.Empty, string.Empty).ExecuteAsync(query, token);

    // ToDo move raw GET access to client.Advanced during public API normalization.
    public Task<HttpResponseMessage> ApiGetAsync(string query, int count, int start = 0, CancellationToken token = default)
        => _httpClient.ApiGetAsync(query, count, start, token: token);
}