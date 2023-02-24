namespace ApiBureau.Bullhorn.Api.Dtos;

public class StatusDto
{
    public string Status { get; }

    public StatusDto(string status) => Status = status;
}