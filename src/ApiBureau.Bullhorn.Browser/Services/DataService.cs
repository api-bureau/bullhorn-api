using ApiBureau.Bullhorn.Api.Http;

namespace ApiBureau.Bullhorn.Browser.Services;

public class DataService
{
    private readonly BullhornClient _client;

    public DataService(BullhornClient client)
    {
        _client = client;
    }

    public async Task<DynamicQueryResponse> ApiCallToDynamicAsync(string query, int count, int start = 0)
    {
        await _client.CheckConnectionAsync();

        var response = await _client.ApiCallToDynamicAsync(query, count, start);

        return response;
    }
}