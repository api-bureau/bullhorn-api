namespace ApiBureau.Bullhorn.Api.Dtos;

public class CountryDto : IdDto
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;
}
