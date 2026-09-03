namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class CountryEndpoint : QueryEndpointBase<CountryDto>
{
    private const string EntityDefaultFields = "id,code,name";

    internal CountryEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Returns all countries
    /// </summary>
    /// <returns></returns>
    public async Task<List<CountryDto>> GetAllAsync() => await QueryWhereAsync();
}