namespace ApiBureau.Bullhorn.Api.Endpoints
{
    public class EntityResponse<T> : ErrorResponseDto
    {
        public T Data { get; set; }
    }
}
