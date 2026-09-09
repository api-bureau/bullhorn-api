namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class CorporateUserEndpoint : QueryEndpointBase<UserDto>
{
    private const string EntityDefaultFields = "id,firstName,lastName,name,isDeleted,departments";

    internal CorporateUserEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Returns all users
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserDto>> GetAllAsync() => await GetWhereAsync();

    /// <summary>
    /// Http POST /entity/CorporateUser/{corporateUserId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse, ErrorResponse>> UpdateAsync(int corporateUserId, object data)
        => HttpClient.PostAsJsonAsync(EntityType.CorporateUser, corporateUserId, data);
}