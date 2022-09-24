namespace ApiBureau.Bullhorn.Api.Endpoints;

public class CorporationDepartmentApi : BaseEndpoint
{
    public static readonly string DefaultFields = "id,dateAdded,enabled,name";

    public CorporationDepartmentApi(ApiConnection apiConnection) : base(apiConnection) { }

    /// <summary>
    /// Returns all departments
    /// </summary>
    /// <returns></returns>
    public async Task<List<CorporationDepartmentDto>> GetAsync(string? fields = null)
    {
        var query = $"CorporationDepartment?fields={fields ?? DefaultFields}&where=id>0";

        return await ApiConnection.QueryAsync<CorporationDepartmentDto>(query) ?? new List<CorporationDepartmentDto>();
    }
}
