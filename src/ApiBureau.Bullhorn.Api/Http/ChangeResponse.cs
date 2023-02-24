namespace ApiBureau.Bullhorn.Api.Http
{
    public class ChangeResponse
    {
        public string? ChangedEntityType { get; set; }
        public int ChangedEntityId { get; set; }
        public string? ChangeType { get; set; }
    }
}