using Microsoft.Extensions.Logging;

namespace ApiBureau.Bullhorn.Api.Console.Services;

public class PlayGroundService
{
    private readonly ILogger<PlayGroundService> _logger;
    private readonly BullhornClient _bullhornApi;

    public PlayGroundService(ILogger<PlayGroundService> logger, BullhornClient bullhornApi)
    {
        _logger = logger;
        _bullhornApi = bullhornApi;
    }

    public async Task TestApiAsync()
    {
        try
        {
            await _bullhornApi.CheckConnectionAsync();
            await GetDepartmentsAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, nameof(TestApiAsync));

            throw;
        }
    }

    //private async Task QueryExampleAsync()
    //{
    //    var testUrl = "Department?fields=id,description,enabled,name&where=id>0";

    //    var result = await _bullhornApi.QueryAsync<DepartmentDto>(testUrl);

    //    _logger.LogInformation("Items: {0}", result.Count);
    //}

    private async Task GetDepartmentsAsync()
    {
        var result = await _bullhornApi.Department.GetAsync();

        _logger.LogInformation("Items: {0}", result.Count);
    }
}
