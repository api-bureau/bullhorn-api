namespace ApiBureau.Bullhorn.Api.Http;

public class ErrorResponse
{
    public string ErrorMessage { get; set; } = "";
    public string ErrorMessageKey { get; set; } = "";
    public int ErrorCode { get; set; }
    public bool Success => string.IsNullOrWhiteSpace(ErrorMessage) && ErrorCode == 0;
}