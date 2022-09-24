namespace ApiBureau.Bullhorn.Api.Dtos;

public class ClientCorporationDto : IdDto
{
    public string Name { get; set; } = null!;
    public string Status { get; set; } = null!;

    public List<UserDto>? Owners { get; set; }

    public ClientCorporationDto() { }
    public ClientCorporationDto(int id) => Id = id;
    public ClientCorporationDto(string name) => Name = name;
}