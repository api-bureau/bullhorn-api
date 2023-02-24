namespace ApiBureau.Bullhorn.Api.Endpoints;

public class CountryEndpoint : QueryBaseEndpoint<CountryDto>
{
    private const string EntityDefaultFields = "id,code,name";

    public CountryEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Returns all coutries
    /// </summary>
    /// <returns></returns>
    public async Task<List<CountryDto>> GetAllCountriesAsync() => await QueryWhereAsync();
}