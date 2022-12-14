namespace ApiBureau.Bullhorn.Api.Endpoints;

public class CountryEndpoint : BaseEndpoint
{
    private const string DefaultFields = "id,code,name";
    private const string DefaultWhere = "id>0";

    public CountryEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    /// <summary>
    /// Returns all countries
    /// </summary>
    /// <returns></returns>
    public Task<List<CountryDto>> GetAsync(string? fields = DefaultFields, string? defaultWhere = DefaultWhere)
        => ApiConnection.QueryAsync<CountryDto>($"{EntityType.Country}?fields={fields}&where={defaultWhere}");
}
