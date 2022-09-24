namespace ApiBureau.Bullhorn.Api.Endpoints;

public class CorporateUserApi : BaseEndpoint
{
    public CorporateUserApi(ApiConnection apiConnection) : base(apiConnection) { }

    /// <summary>
    /// Returns all users
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserDto>> GetAsync()
    {
        const string query = "CorporateUser?fields=id,firstName,lastName,name,isDeleted,departments&where=id>0";

        return await ApiConnection.QueryAsync<UserDto>(query);
    }
}
