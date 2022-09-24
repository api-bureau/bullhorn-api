namespace ApiBureau.Bullhorn.Api.Endpoints;

public class AppointmentEndpoint : BaseEndpoint
{
    public AppointmentEndpoint(ApiConnection apiConnection) : base(apiConnection) { }

    public readonly string DefaultFields = "id,candidateReference,clientContactReference,dateAdded,dateBegin,dateLastModified,type,isDeleted,jobOrder,owner";

    public async Task<List<AppointmentDto>> GetAsync(long timestampFrom)
    {
        var query = $"Appointment?fields={DefaultFields}&where=dateAdded>{timestampFrom} AND candidateReference IS NOT NULL";

        return await ApiConnection.QueryAsync<AppointmentDto>(query);
    }

    public async Task<List<AppointmentDto>> GetAsync(long timestampFrom, long timestampTo)
    {
        var query = $"Appointment?fields={DefaultFields}&where=dateAdded>{timestampFrom} AND dateAdded<{timestampTo} AND candidateReference IS NOT NULL";

        return await ApiConnection.QueryAsync<AppointmentDto>(query);
    }

    public async Task<List<AppointmentDto>> GetNewAndUpdatedFromAsync(long timestampFrom)
    {
        var query = $"Appointment?fields={DefaultFields}&where=(dateAdded>{timestampFrom} OR dateLastModified>{timestampFrom}) AND candidateReference IS NOT NULL";

        return await ApiConnection.QueryAsync<AppointmentDto>(query);
    }

    public Task<HttpResponseMessage> AddAsync(AppointmentDto appointment)
        => ApiConnection.PutAsJsonAsync(EntityType.Appointment, appointment);
}
