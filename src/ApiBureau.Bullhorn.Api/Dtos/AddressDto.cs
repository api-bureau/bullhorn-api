namespace ApiBureau.Bullhorn.Api.Dtos;

public class AddressDto
{
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? CountryName { get; set; }
    public string? CountryCode { get; set; }

    [JsonPropertyName("countryID")]
    public int? CountryId { get; set; }


    public AddressDto() { }
    public AddressDto(int? countryId) => CountryId = countryId;
}
