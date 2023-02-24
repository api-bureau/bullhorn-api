namespace ApiBureau.Bullhorn.Api.Dtos;

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