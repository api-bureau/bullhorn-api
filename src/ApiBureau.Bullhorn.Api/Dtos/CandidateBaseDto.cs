namespace ApiBureau.Bullhorn.Api.Dtos;

public class CandidateBaseDto : EntityBaseDto
{
    public string? CompanyName { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Email2 { get; set; }
    public string? Email3 { get; set; }
    public string? Mobile { get; set; }
    public string? Phone { get; set; }
    public string? Phone2 { get; set; }
    public string? Phone3 { get; set; }
    public string? WorkPhone { get; set; }
    public bool IsDeleted { get; set; }
    public string Status { get; set; } = null!;
    public long CustomDate2 { get; set; }
    public string? Source { get; set; }
    public UserDto Owner { get; set; }
    public AddressDto Address { get; set; }

    public CandidateBaseDto()
    {
        Owner = new UserDto();
        Address = new AddressDto();
    }
}