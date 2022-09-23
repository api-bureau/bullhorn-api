namespace ApiBureau.Bullhorn.Api.Endpoints
{
    public class ChangeResponse
    {
        public int ChangedEntityId { get; set; }
        public string ChangedEntityType { get; set; } = "";
        public string ChangedType { get; set; } = "";
    }
}
