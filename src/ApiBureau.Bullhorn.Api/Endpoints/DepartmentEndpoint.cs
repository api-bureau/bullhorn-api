namespace ApiBureau.Bullhorn.Api.Endpoints;

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
