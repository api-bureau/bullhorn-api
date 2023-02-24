namespace ApiBureau.Bullhorn.Api.Endpoints;

public class QueryResponse<T> : ErrorResponseDto
{
    public int Total { get; set; }
    public int Start { get; set; }
    public int Count { get; set; }
    public List<T> Data { get; set; } = new List<T>();
}