namespace ApiBureau.Bullhorn.Api.Http;

public class ErrorResponse
{
    public string ErrorMessage { get; set; } = "";
    public string ErrorMessageKey { get; set; } = "";
    public int ErrorCode { get; set; }
    public List<ErrorDetail>? Errors { get; set; }
    public int? HttpStatusCode { get; set; }
    public string? ReasonPhrase { get; set; }
    public string? ErrorsFormatted => Errors?.Count > 0
        ? string.Join("; ", Errors.Select(e => $"{e.PropertyName}: {e.DetailMessage}"))
        : null;
    public string Message => string.Join("; ", new[] { ErrorMessage, ErrorsFormatted, ReasonPhrase }
        .Where(message => !string.IsNullOrWhiteSpace(message)));

    public static ErrorResponse FromMessage(string? message, HttpResponseMessage? response = null) => new()
    {
        ErrorMessage = message ?? response?.ReasonPhrase ?? "The Bullhorn request failed.",
        HttpStatusCode = response is null ? null : (int)response.StatusCode,
        ReasonPhrase = response?.ReasonPhrase
    };

    public override string ToString() => Message;
}

public class ErrorDetail
{
    public string? PropertyName { get; set; }
    public string? Severity { get; set; }
    public string? DetailMessage { get; set; }
    public string? Type { get; set; }
}