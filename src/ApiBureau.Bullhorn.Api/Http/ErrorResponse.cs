namespace ApiBureau.Bullhorn.Api.Http;

public class ErrorResponse
{
    public string ErrorMessage { get; set; } = "";
    public string ErrorMessageKey { get; set; } = "";
    public int ErrorCode { get; set; }
    public List<ErrorDetail>? Errors { get; set; }
    public bool Success => string.IsNullOrWhiteSpace(ErrorMessage) && ErrorCode == 0;
}

public class ErrorDetail
{
    public string? PropertyName { get; set; }
    public string? Severity { get; set; }
    public string? DetailMesssage { get; set; }
    public string? Type { get; set; }
}