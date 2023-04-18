namespace ApiBureau.Bullhorn.Api.Http;

public class LoginResponse : ErrorResponse
{
    public string? BhRestToken { get; set; }

    public string? RestUrl { get; set; }

    public bool IsValid => BhRestToken != null && RestUrl != null;
}