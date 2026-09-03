namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class CorporationDepartmentEndpoint : QueryEndpointBase<CorporationDepartmentDto>
{
    private const string EntityDefaultFields = "id,dateAdded,enabled,name";

    internal CorporationDepartmentEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Returns all departments
    /// </summary>
    /// <returns></returns>
    public async Task<List<CorporationDepartmentDto>> GetAllDepartmentsAsync() => await QueryWhereAsync();
}