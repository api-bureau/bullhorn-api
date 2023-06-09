namespace ApiBureau.Bullhorn.Api.Http;

public class ChangeResponse
{
    public string? ChangedEntityType { get; set; }
    public int ChangedEntityId { get; set; }
    public string? ChangeType { get; set; }
    public List<ChangeResponseMessage>? Messages { get; set; }
}

public class ChangeResponseMessage
{
    public string? DetailMessage { get; set; }
    public string? PropertyName { get; set; }
    public string? Severity { get; set; }
    public string? Type { get; set; }
}