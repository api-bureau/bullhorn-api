namespace ApiBureau.Bullhorn.Api.Http;

public class EntityResponse<T>
{
    public T Data { get; set; } = default!;
}