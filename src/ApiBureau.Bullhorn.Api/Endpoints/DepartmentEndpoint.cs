namespace ApiBureau.Bullhorn.Api.Endpoints;

public class DepartmentEndpoint : QueryEndpointBase<DepartmentDto>
{
    private const string EntityDefaultFields = "id,description,enabled,name";

    public DepartmentEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Returns all departments
    /// </summary>
    /// <returns></returns>
    public async Task<List<DepartmentDto>> GetAllAsync() => await QueryWhereAsync();
}