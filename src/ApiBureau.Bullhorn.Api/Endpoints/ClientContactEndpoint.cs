using ApiBureau.Bullhorn.Api.Internals;

namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class ClientContactEndpoint : SearchEndpointBase<ClientContactDto>
{
    private const string EntityDefaultFields = "id,clientCorporation,isDeleted,firstName,lastName,email,dateAdded,dateLastModified,owner";

    internal ClientContactEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    public Task<Result<ChangeResponse>> AddAsync(ClientContactDto dto, CancellationToken cancellationToken)
        => HttpClient.PutAsJsonAsync(EntityType.ClientContact, dto, cancellationToken);

    public async Task<List<ClientContactDto>> FindClientContactIdByEmailAsync(List<string> emails, CancellationToken cancellationToken)
    {
        var query = $"{RequestUrl}?fields=id,firstName,lastName,email&query=email:({BullhornQuery.QuoteAny(emails)}) AND isDeleted:0";

        return await ExecuteSearchAsync(query, cancellationToken);
    }

    /// <summary>
    /// Http POST /entity/ClientContact/{clientContactId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int clientContactId, object data, CancellationToken cancellationToken)
        => HttpClient.PostAsJsonAsync(EntityType.ClientContact, clientContactId, data, cancellationToken);
}