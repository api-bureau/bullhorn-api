namespace ApiBureau.Bullhorn.Api.Endpoints
{
    public class DynamicResponse : ErrorResponseDto
    {
        public int Total { get; set; }
        public string Json { get; set; } = "";
        public string RequestUri { get; set; } = "";
    }
}