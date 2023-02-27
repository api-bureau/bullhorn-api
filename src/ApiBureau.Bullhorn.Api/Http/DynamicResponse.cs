namespace ApiBureau.Bullhorn.Api.Http;

public class DynamicResponse : ErrorResponse
{
    public int Total { get; set; }
    public string Json { get; set; } = "";
    public string RequestUri { get; set; } = "";
}