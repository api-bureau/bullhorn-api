namespace ApiBureau.Bullhorn.Api.Http;

public class QueryResponse<T> : ErrorResponse
{
    public int Total { get; set; }
    public int Start { get; set; }
    public int Count { get; set; }
    public List<T> Data { get; set; } = new List<T>();
}