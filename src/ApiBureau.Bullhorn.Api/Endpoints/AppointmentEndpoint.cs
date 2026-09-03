namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class AppointmentEndpoint : QueryEndpointBase<AppointmentDto>
{
    private const string EntityDefaultFields = "id,candidateReference,clientContactReference,dateAdded,dateBegin,dateLastModified,type,isDeleted,jobOrder,owner";

    internal AppointmentEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    [Obsolete("Use QueryFromAsync", true)]
    public async Task<List<AppointmentDto>> GetAsync(long timestampFrom, CancellationToken token)
    {
        var query = $"{RequestUrl}?fields={DefaultFields}&where=dateAdded>{timestampFrom} AND candidateReference IS NOT NULL";

        return await ExecuteQueryAsync(query, token);
    }

    [Obsolete("Use QueryFromToAsync", true)]
    public async Task<List<AppointmentDto>> GetAsync(long timestampFrom, long timestampTo, CancellationToken token)
    {
        var query = $"{RequestUrl}?fields={DefaultFields}&where=dateAdded>{timestampFrom} AND dateAdded<{timestampTo} AND candidateReference IS NOT NULL";

        return await ExecuteQueryAsync(query, token);
    }

    [Obsolete("Use QueryNewAndUpdatedFromAsync", true)]
    public async Task<List<AppointmentDto>> GetNewAndUpdatedFromAsync(long timestampFrom, CancellationToken token)
    {
        var query = $"{RequestUrl}?fields={DefaultFields}&where=(dateAdded>{timestampFrom} OR dateLastModified>{timestampFrom}) AND candidateReference IS NOT NULL";

        return await ExecuteQueryAsync(query, token);
    }

    public Task<Result<ChangeResponse>> AddAsync(NewAppointmentDto appointment, CancellationToken token)
        => HttpClient.PutAsJsonAsync(EntityType.Appointment, appointment, token);

    /// <summary>
    /// Http POST /entity/Appointment/{appointmentId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int appointmentId, object data)
        => HttpClient.PostAsJsonAsync(EntityType.Appointment, appointmentId, data);
}