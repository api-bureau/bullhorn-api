namespace ApiBureau.Bullhorn.Api.Dtos;

public class TargetEntityDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    [JsonPropertyName("_subtype")]
    public string? Subtype { get; set; }
}