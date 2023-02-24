using System.Text.Json;

namespace ApiBureau.Bullhorn.Api.Http;

public class QueryResponse : ErrorResponse
{
    public int Total { get; set; }
    public int Start { get; set; }
    public int Count { get; set; }

    //ToDo Json
    public List<JsonDocument> Data { get; set; }
}