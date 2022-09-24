namespace ApiBureau.Bullhorn.Api.Endpoints;

public class ContryEndpoint : BaseEndpoint
{
    public ContryEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    /// <summary>
    /// Returns all countries
    /// </summary>
    /// <returns></returns>
    public Task<List<CountryDto>> GetAsync()
    {
        return ApiConnection.QueryAsync<CountryDto>("Country?fields=id,code,name&where=id>0");
    }
}
