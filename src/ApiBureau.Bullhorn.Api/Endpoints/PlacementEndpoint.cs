namespace ApiBureau.Bullhorn.Api.Endpoints;

/// <summary>
/// Provides query and mutation operations for Bullhorn placements.
/// </summary>
public sealed class PlacementEndpoint : QueryEndpointBase<PlacementDto>
{
    private const string EntityDefaultFields = "id,billingClientContact,candidate,dateAdded,dateLastModified,dateBegin,dateEnd,employeeType,employmentType,fee,flatFee,jobOrder,payRate,correlatedCustomText1,salary,salaryUnit,status";

    internal PlacementEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    /// <summary>
    /// Approves the specified placement.
    /// </summary>
    /// <param name="placementId">The Bullhorn placement identifier.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>The HTTP response returned by the Bullhorn approval endpoint.</returns>
    public Task<HttpResponseMessage> ApproveAsync(int placementId, CancellationToken cancellationToken = default)
        => HttpClient.PostAsync($"services/{RequestUrl}/approve/{placementId}", null, cancellationToken);

    /// <summary>
    /// Updates an existing placement.
    /// </summary>
    /// <param name="placementId">The Bullhorn placement identifier.</param>
    /// <param name="data">The fields and values to update.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the update request.</returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int placementId, object data, CancellationToken cancellationToken = default)
        => HttpClient.PostAsJsonAsync(EntityType.Placement, placementId, data, cancellationToken);

    /// <summary>
    /// Creates a new placement.
    /// </summary>
    /// <param name="content">The placement payload to create.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>The Bullhorn change response for the create request.</returns>
    public Task<Result<ChangeResponse>> AddAsync(object content, CancellationToken cancellationToken)
        => HttpClient.PutAsJsonAsync(EntityType.Placement, content, cancellationToken);
}