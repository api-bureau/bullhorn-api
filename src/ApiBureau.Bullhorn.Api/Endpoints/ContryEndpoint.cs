namespace ApiBureau.Bullhorn.Api.Endpoints;

public class ContryEndpoint : BaseEndpoint
{
    public ContryEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    /// <summary>
    /// Returns all countries
    /// </summary>
    /// <returns></returns>
    public Task<List<CountryDto>> GetAsync()
        => ApiConnection.QueryAsync<CountryDto>("Country?fields=id,code,name&where=id>0");
}

public class DepartmentEndpoint : BaseEndpoint
{
    public DepartmentEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    /// <summary>
    /// Returns all departments
    /// </summary>
    /// <returns></returns>
    public Task<List<DepartmentDto>> GetAsync()
        => ApiConnection.QueryAsync<DepartmentDto>("Department?fields=id,description,enabled,name&where=id>0");
}
