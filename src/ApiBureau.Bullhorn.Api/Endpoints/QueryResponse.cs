using System.Text.Json;

namespace ApiBureau.Bullhorn.Api.Endpoints;

public class QueryResponse : ErrorResponseDto
{
    public int Total { get; set; }
    public int Start { get; set; }
    public int Count { get; set; }

    //ToDo Json
    public List<JsonDocument> Data { get; set; }
}