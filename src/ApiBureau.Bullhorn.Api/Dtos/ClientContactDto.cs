namespace ApiBureau.Bullhorn.Api.Dtos;

public class ClientContactDto : EntityBaseDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public UserDto Owner { get; set; }
    public ClientCorporationDto ClientCorporation { get; set; } = new ClientCorporationDto();
    public AddressDto Address { get; set; } = new();
    public bool IsDeleted { get; set; }

    public ClientContactDto()
    {
        Owner = new UserDto();
    }
}
