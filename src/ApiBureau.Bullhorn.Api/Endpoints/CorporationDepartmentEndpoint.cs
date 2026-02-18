namespace ApiBureau.Bullhorn.Api.Endpoints;

public class CorporationDepartmentEndpoint : QueryEndpointBase<CorporationDepartmentDto>
{
    private const string EntityDefaultFields = "id,dateAdded,enabled,name";

    public CorporationDepartmentEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Returns all departments
    /// </summary>
    /// <returns></returns>
    public async Task<List<CorporationDepartmentDto>> GetAllDepartmentsAsync() => await QueryWhereAsync();
}