namespace ApiBureau.Bullhorn.Api.Dtos;

public class UserDto : IdDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Name { get; set; }
    public bool IsDeleted { get; set; }

    [JsonPropertyName("_subtype")]
    public string? Subtype { get; set; }

    public DepartmentsDto? Departments { get; set; }

    public UserDto() { }

    public UserDto(int id) => Id = id;
}
