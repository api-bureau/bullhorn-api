using ApiBureau.Bullhorn.Api.Interfaces;

namespace ApiBureau.Bullhorn.Browser.Services;

public class DataService
{
    private readonly IBullhornClient _client;

    public DataService(IBullhornClient client)
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