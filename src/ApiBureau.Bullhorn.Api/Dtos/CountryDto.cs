namespace ApiBureau.Bullhorn.Api.Dtos;

public class CountryDto : IdDto
{
    public string? Code { get; set; }

    public string Name { get; set; } = string.Empty;
}