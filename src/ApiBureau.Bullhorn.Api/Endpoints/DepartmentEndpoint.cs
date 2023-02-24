namespace ApiBureau.Bullhorn.Api.Endpoints;

public class DepartmentEndpoint : QueryBaseEndpoint<DepartmentDto>
{
    private const string EntityDefaultFields = "id,description,enabled,name";

    public DepartmentEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Returns all departments
    /// </summary>
    /// <returns></returns>
    public async Task<List<DepartmentDto>> GetAllDepertmentsAsync() => await QueryWhereAsync();
}