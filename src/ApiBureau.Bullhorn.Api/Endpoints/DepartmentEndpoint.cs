namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class DepartmentEndpoint : QueryEndpointBase<DepartmentDto>
{
    private const string EntityDefaultFields = "id,description,enabled,name";

    internal DepartmentEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Returns all departments
    /// </summary>
    /// <returns></returns>
    public async Task<List<DepartmentDto>> GetAllAsync() => await GetWhereAsync();
}