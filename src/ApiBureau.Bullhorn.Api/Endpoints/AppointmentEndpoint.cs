namespace ApiBureau.Bullhorn.Api.Endpoints;

public class AppointmentEndpoint : QueryBaseEndpoint<AppointmentDto>
{
    private const string EntityDefaultFields = "id,candidateReference,clientContactReference,dateAdded,dateBegin,dateLastModified,type,isDeleted,jobOrder,owner";

    public AppointmentEndpoint(ApiConnection apiConnection, string requestUrl) : base(apiConnection, requestUrl, EntityDefaultFields) { }

    [Obsolete("Use QueryFromAsync", true)]
    public async Task<List<AppointmentDto>> GetAsync(long timestampFrom, CancellationToken token)
    {
        var query = $"{RequestUrl}?fields={DefaultFields}&where=dateAdded>{timestampFrom} AND candidateReference IS NOT NULL";

        return await ApiConnection.QueryAsync<AppointmentDto>(query, token);
    }

    [Obsolete("Use QueryFromToAsync", true)]
    public async Task<List<AppointmentDto>> GetAsync(long timestampFrom, long timestampTo, CancellationToken token)
    {
        var query = $"{RequestUrl}?fields={DefaultFields}&where=dateAdded>{timestampFrom} AND dateAdded<{timestampTo} AND candidateReference IS NOT NULL";

        return await ApiConnection.QueryAsync<AppointmentDto>(query, token);
    }

    [Obsolete("Use QueryNewAndUpdatedFromAsync", true)]
    public async Task<List<AppointmentDto>> GetNewAndUpdatedFromAsync(long timestampFrom, CancellationToken token)
    {
        var query = $"{RequestUrl}?fields={DefaultFields}&where=(dateAdded>{timestampFrom} OR dateLastModified>{timestampFrom}) AND candidateReference IS NOT NULL";

        return await ApiConnection.QueryAsync<AppointmentDto>(query, token);
    }

    public Task<Result<ChangeResponse>> AddAsync(NewAppointmentDto appointment)
        => ApiConnection.PutAsJsonAsync(EntityType.Appointment, appointment);

    /// <summary>
    /// Http POST /entity/Appointment/{appointmentId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int appointmentId, object data)
        => ApiConnection.PostAsJsonAsync(EntityType.Appointment, appointmentId, data);
}