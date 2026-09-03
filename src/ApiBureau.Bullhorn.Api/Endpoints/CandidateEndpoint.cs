using ApiBureau.Bullhorn.Api.Internals;
using System.Text;
using System.Text.Json;

namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class CandidateEndpoint : SearchEndpointBase<CandidateDto>
{
    private const string EntityDefaultFields = "id,status,isDeleted,firstName,lastName,email,dateAdded,dateLastModified,source,owner";

    internal CandidateEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    public async Task AddOldAsync(CandidateDto dto, CancellationToken token)
        => await HttpClient.ApiPutAsync($"entity/{RequestUrl}", new StringContent(JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), Encoding.UTF8, "application/json"), token);

    /// <summary>
    /// Adds a new candidate, sending all fields from the provided <see cref="CandidateDto"/>.
    /// </summary>
    /// <remarks>
    /// Use this overload only when all candidate fields should be populated. Any unset properties will be
    /// serialised as empty or default values, which will overwrite existing data in Bullhorn. To set only
    /// a subset of fields, use <see cref="AddAsync(object, CancellationToken)"/> instead.
    /// </remarks>
    /// <param name="dto">The candidate data to add. All properties are serialised and sent to the API.</param>
    /// <param name="token">A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result{ChangeResponse}"/> indicating the outcome of the operation.</returns>
    public async Task<Result<ChangeResponse>> AddAsync(CandidateDto dto, CancellationToken token)
        => await HttpClient.PutAsJsonAsync(EntityType.Candidate, dto, token);

    /// <summary>
    /// Adds a new candidate using an anonymous or partial object, sending only the properties defined on it.
    /// </summary>
    /// <remarks>
    /// Prefer this overload when only a subset of candidate fields need to be set, avoiding empty values
    /// overwriting existing Bullhorn data. The object is serialised as-is to the API.
    /// </remarks>
    /// <param name="dto">An object whose properties represent the candidate fields to add.</param>
    /// <param name="token">A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result{ChangeResponse}"/> indicating the outcome of the operation.</returns>
    public async Task<Result<ChangeResponse>> AddAsync(object dto, CancellationToken token)
        => await HttpClient.PutAsJsonAsync(EntityType.Candidate, dto, token);

    public async Task<int> GetFilesCount(int id, CancellationToken token)
    {
        var query = $"entity/{RequestUrl}/{id}/fileAttachments?fields=id";

        var response = await HttpClient.GetAsync(query, token);

        return (await response.DeserializeAsync<QueryResponse>())?.Total ?? 0;
    }

    public async Task<CandidateDto?> FindCandidateIdByEmailAsync(string email, CancellationToken token)
    {
        var query = $"search/{RequestUrl}?fields=id,firstName,lastName&query=email:\"{email}\" AND isDeleted:0";

        var response = await HttpClient.GetAsync(query, token);

        var searchResponse = await response.DeserializeAsync<SearchResponse2<CandidateDto>>();

        return searchResponse?.Data?.FirstOrDefault();
    }

    public async Task<List<CandidateDto>> FindCandidateIdByEmailAsync(List<string> emails, CancellationToken token)
    {
        var query = $"{RequestUrl}?fields=id,firstName,lastName,email&query=email:({BullhornQuery.QuoteAny(emails)}) AND isDeleted:0";

        return await ExecuteSearchAsync(query, token);
    }

    /// <summary>
    /// Http POST /entity/Candidate/{candidateId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int candidateId, object data, CancellationToken token)
        => HttpClient.PostAsJsonAsync(EntityType.Candidate, candidateId, data, token);
}