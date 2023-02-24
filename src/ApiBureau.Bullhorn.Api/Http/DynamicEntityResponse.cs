namespace ApiBureau.Bullhorn.Api.Http
{
    public class DynamicEntityResponse : DynamicResponse
    {
        public dynamic Data { get; set; }
        public dynamic DynamicData { get; set; }
    }
}