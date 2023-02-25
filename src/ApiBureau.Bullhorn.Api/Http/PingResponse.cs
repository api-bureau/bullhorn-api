namespace ApiBureau.Bullhorn.Api.Http;

public class PingResponse
{
    public long SessionExpires { get; set; }

    public DateTime SessionExpiryDate => SessionExpires.ToDateTime();

    // 30 seconds added for security
    public bool Valid => SessionExpiryDate > DateTime.Now.AddSeconds(30);

    //public SessionDto() => SessionExpires = DateTime.Now.AddYears(-100).Timestamp();

    public void SetExpiryDate(long expiryDate) => SessionExpires = expiryDate;
}