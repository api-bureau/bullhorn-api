using ApiBureau.Bullhorn.Api.Endpoints;

namespace ApiBureau.Bullhorn.Api.Interfaces;

public interface IBullhornClient
{
    BullhornAdvancedClient Advanced { get; }
    AppointmentEndpoint Appointments { get; }
    CandidateEndpoint Candidates { get; }
    CandidateWorkHistoryEndpoint CandidateWorkHistories { get; }
    ClientContactEndpoint ClientContacts { get; }
    ClientCorporationEndpoint ClientCorporations { get; }
    CorporateUserEndpoint CorporateUsers { get; }
    CorporationDepartmentEndpoint CorporationDepartments { get; }
    CountryEndpoint Countries { get; }
    DepartmentEndpoint Departments { get; }
    EntityEditHistoryEndpoint EntityEditHistories { get; }
    EventEndpoint Events { get; }
    FileEndpoint Files { get; }
    FileAttachmentEndpoint FileAttachments { get; }
    JobOrderEndpoint JobOrders { get; }
    JobSubmissionEndpoint JobSubmissions { get; }
    JobSubmissionHistoryEndpoint JobSubmissionHistories { get; }
    MassUpdateEndpoint MassUpdates { get; }
    NoteEndpoint Notes { get; }
    OpportunityEndpoint Opportunities { get; }
    PlacementEndpoint Placements { get; }
    PlacementChangeRequestEndpoint PlacementChangeRequests { get; }
    PlacementCommissionEndpoint PlacementCommissions { get; }
    ResumeEndpoint Resumes { get; }
    SendoutEndpoint Sendouts { get; }

    /// <summary>
    /// Check the Bullhorn client connection.
    /// </summary>
    Task<bool> CheckConnectionAsync(IProgress<string>? progress = null);
}