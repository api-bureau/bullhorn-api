using ApiBureau.Bullhorn.Api.Core;

namespace ApiBureau.Bullhorn.Api.Console.Services;

/// <summary>
/// Example of how to fetch entities from Bullhorn
/// </summary>
public class PlayGroundService
{
    private readonly ILogger<PlayGroundService> _logger;
    private readonly BullhornClient _bullhornApi;

    public PlayGroundService(ILogger<PlayGroundService> logger, BullhornClient bullhornApi)
    {
        _logger = logger;
        _bullhornApi = bullhornApi;
    }

    public async Task GetEntitiesAsync()
    {
        try
        {
            await _bullhornApi.CheckConnectionAsync();

            //await GetDepartmentsAsync();
            await QueryExampleAsync();
            //await GetCountriesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Entities fetch failed");

            throw;
        }
    }

    private async Task QueryExampleAsync()
    {
        var result = await _bullhornApi.QueryAsync<DepartmentDto>($"{EntityType.Department}?fields=id,name&where=id>0");

        _logger.LogInformation("Items: {count}", result.Count);
    }

    private async Task GetDepartmentsAsync()
    {
        var result = await _bullhornApi.Department.GetAllAsync();

        _logger.LogInformation("Items: {count}", result.Count);
    }

    private async Task GetCountriesAsync()
    {
        var result = await _bullhornApi.Country.GetAllAsync();

        _logger.LogInformation("Items: {count}", result.Count);
    }
}
