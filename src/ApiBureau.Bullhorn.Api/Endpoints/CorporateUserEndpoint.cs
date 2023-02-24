namespace ApiBureau.Bullhorn.Api.Endpoints;

public class CorporateUserEndpoint : QueryBaseEndpoint<UserDto>
{
    private const string EntityDefaultFields = "id,firstName,lastName,name,isDeleted,departments";

    public CorporateUserEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Returns all users
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserDto>> GetAllUsersAsync() => await QueryWhereAsync();
}