namespace ApiBureau.Bullhorn.Browser.Services;

public class DataService
{
    private readonly BullhornClient _client;

    public DataService(BullhornClient client)
    {
        _client = client;
    }

    public async Task<HttpResponseMessage> GetAsync(string query, int count, int start = 0)
    {
        await _client.CheckConnectionAsync();

        var response = await _client.ApiGetAsync(query, count, start);

        return response;
    }
}