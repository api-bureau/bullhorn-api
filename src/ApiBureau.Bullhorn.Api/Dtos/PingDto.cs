using ApiBureau.Bullhorn.Api.Extensions;

namespace ApiBureau.Bullhorn.Api.Dtos;

public class PingDto
{
    public long SessionExpires { get; set; }

    public DateTime SessionExpiryDate => SessionExpires.ToDateTime();

    // 30 seconds added for security
    public bool Valid => SessionExpiryDate > DateTime.Now.AddSeconds(30);

    //public SessionDto() => SessionExpires = DateTime.Now.AddYears(-100).Timestamp();

    public void SetExpiryDate(long expiryDate) => SessionExpires = expiryDate;
}
