namespace ApiBureau.Bullhorn.Api.Http;

public class EntityResponse<T> : ErrorResponse
{
    public T Data { get; set; }
}