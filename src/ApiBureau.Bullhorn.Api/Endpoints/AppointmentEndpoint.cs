namespace ApiBureau.Bullhorn.Api.Endpoints;

public sealed class AppointmentEndpoint : QueryEndpointBase<AppointmentDto>
{
    private const string EntityDefaultFields = "id,candidateReference,clientContactReference,dateAdded,dateBegin,dateLastModified,type,isDeleted,jobOrder,owner";

    internal AppointmentEndpoint(BullhornHttpClient httpClient, string requestUrl)
        : base(httpClient, requestUrl, EntityDefaultFields) { }

    public Task<Result<ChangeResponse>> AddAsync(NewAppointmentDto appointment, CancellationToken cancellationToken)
        => HttpClient.PutAsJsonAsync(EntityType.Appointment, appointment, cancellationToken);

    /// <summary>
    /// Http POST /entity/Appointment/{appointmentId}
    /// </summary>
    /// <returns></returns>
    public Task<Result<ChangeResponse>> UpdateAsync(int appointmentId, object data)
        => HttpClient.PostAsJsonAsync(EntityType.Appointment, appointmentId, data);
}