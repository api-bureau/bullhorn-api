namespace ApiBureau.Bullhorn.Api.Endpoints;

public class CountryEndpoint : BaseEndpoint
{
    private const string DefaultFields = "id,code,name";

    public CountryEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    /// <summary>
    /// Returns all countries
    /// </summary>
    /// <returns></returns>
    public Task<List<CountryDto>> GetAsync(string? fields = DefaultFields)
        => ApiConnection.QueryAsync<CountryDto>($"{EntityType.Country}?fields={fields ?? DefaultFields}&where=id>0");
}
