namespace ApiBureau.Bullhorn.Api.Dtos
{
    public class EventSubscribeDto : ErrorResponseDto
    {
        public long CreatedOn { get; set; }
        public int LastRequestId { get; set; }
        public string SubscriptionId { get; set; } = "";
        public string JmsSelector { get; set; } = "";
    }
}
