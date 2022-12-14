namespace ApiBureau.Bullhorn.Api.Endpoints
{
    public class LoginResponse
    {
        public string? BhRestToken { get; set; }

        public string? RestUrl { get; set; }

        public bool IsValid => BhRestToken != null && RestUrl != null;
    }
}
