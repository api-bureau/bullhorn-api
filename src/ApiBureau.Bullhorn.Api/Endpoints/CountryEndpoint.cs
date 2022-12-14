namespace ApiBureau.Bullhorn.Api.Endpoints;

public class CountryEndpoint : BaseEndpoint
{
    public CountryEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    /// <summary>
    /// Returns all countries
    /// </summary>
    /// <returns></returns>
    public Task<List<CountryDto>> GetAsync()
        => ApiConnection.QueryAsync<CountryDto>("Country?fields=id,code,name&where=id>0");
}
