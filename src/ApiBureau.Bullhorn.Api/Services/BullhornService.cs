namespace ApiBureau.Bullhorn.Api.Services;

/// <summary>
/// Register this as a Singleton in DI if you want to maintain the connection
/// </summary>
public class BullhornService
{
    public IBullhornClient BullhornApi { get; }

    public BullhornService(IBullhornClient bullhornApi) => BullhornApi = bullhornApi;

    /// <summary>
    /// Checks Bullhorn data connection is working. Get latest Candidates added in last X hours.
    /// </summary>
    /// <param name="hours"></param>
    /// <returns></returns>
    public async Task<List<CandidateDto>> BullhornCheck(int hours = 2)
    {
        await CheckConnectionAsync();

        var newCandidates = await BullhornApi.Candidate.SearchFromAsync(DateTime.Now.AddHours(-hours));

        return newCandidates;
    }

    public Task CheckConnectionAsync() => BullhornApi.CheckConnectionAsync();
}