namespace ApiBureau.Bullhorn.Api.Dtos;

public class EventsDto
{
    public List<EventDto> Events { get; set; }
    public int RequestId { get; set; }
    public EventsDto() => Events = new List<EventDto>();
}