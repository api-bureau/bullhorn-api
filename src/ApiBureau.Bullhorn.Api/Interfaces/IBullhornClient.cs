namespace ApiBureau.Bullhorn.Api.Interfaces;

public interface IBullhornClient
{
    AppointmentEndpoint Appointment { get; }
    CandidateEndpoint Candidate { get; }
    CandidateWorkHistoryEndpoint CandidateWorkHistory { get; }
    ClientContactEndpoint ClientContact { get; }
    ClientCorporationEndpoint ClientCorporation { get; }
    CorporateUserEndpoint CorporateUser { get; }
    CorporationDepartmentEndpoint CorporationDepartment { get; }
    CountryEndpoint Country { get; }
    DepartmentEndpoint Department { get; }
    EntityEditHistoryEndpoint EntityEditHistory { get; }
    EventEndpoint Event { get; }
    FileEndpoint File { get; }
    FileAttachmentEndpoint FileAttachment { get; }
    JobOrderEndpoint JobOrder { get; }
    JobSubmissionEndpoint JobSubmission { get; }
    JobSubmissionHistoryEndpoint JobSubmissionHistory { get; }
    MassUpdateEndpoint MassUpdate { get; }
    NoteEndpoint Note { get; }
    OpportunityEndpoint Opportunity { get; }
    PlacementEndpoint Placement { get; }
    PlacementChangeRequestEndpoint PlacementChangeRequest { get; }
    PlacementCommissionEndpoint PlacementCommission { get; }
    ResumeEndpoint Resume { get; }
    SendoutEndpoint Sendout { get; }

    Task<HttpResponseMessage> ApiGetAsync(string query, int count, int start = 0);
    Task CheckConnectionAsync();
    Task<List<T>> QueryAsync<T>(string query);
}