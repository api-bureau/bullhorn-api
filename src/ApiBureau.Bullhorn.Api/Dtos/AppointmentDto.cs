namespace ApiBureau.Bullhorn.Api.Dtos;

/// <summary>
/// Used for updating and retrieving appointments
/// </summary>
public class AppointmentDto : EntityBaseDto
{
    public CandidateDto? CandidateReference { get; set; }
    public ClientContactDto? ClientContactReference { get; set; }
    public JobOrderDto? JobOrder { get; set; }
    public string Type { get; set; }
    public string CommunicationMethod { get; set; }
    public string Subject { get; set; }
    public bool IsDeleted { get; set; }
    public UserDto Owner { get; set; }

    /// <summary>
    /// Timestamp
    /// </summary>
    public long DateBegin { get; set; }

    /// <summary>
    /// Timestamp
    /// </summary>
    public long DateEnd { get; set; }

    public AppointmentDto(int ownerId, string type, string communicationMethod, string subject = "")
    {
        Owner = new UserDto(ownerId);
        Type = type;
        CommunicationMethod = communicationMethod;
        Subject = subject;
    }
}

/// <summary>
/// Used for creating new appointments. Does not have an Id, and does not have a dateAdded or dateLastModified
/// </summary>
public class NewAppointmentDto
{
    public CandidateDto? CandidateReference { get; set; }
    public ClientContactDto? ClientContactReference { get; set; }
    public JobOrderDto? JobOrder { get; set; }
    public string Type { get; set; }
    public string CommunicationMethod { get; set; }
    public string Subject { get; set; }
    public bool IsDeleted { get; set; }
    public UserDto Owner { get; set; }

    /// <summary>
    /// Timestamp
    /// </summary>
    public long DateBegin { get; set; }

    /// <summary>
    /// Timestamp
    /// </summary>
    public long DateEnd { get; set; }

    public NewAppointmentDto(int ownerId, string type, string communicationMethod, string subject = "")
    {
        Owner = new UserDto(ownerId);
        Type = type;
        CommunicationMethod = communicationMethod;
        Subject = subject;
    }
}