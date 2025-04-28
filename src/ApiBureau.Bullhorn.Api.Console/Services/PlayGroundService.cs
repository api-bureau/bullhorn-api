using ApiBureau.Bullhorn.Api.Core;
using ApiBureau.Bullhorn.Api.Interfaces;

namespace ApiBureau.Bullhorn.Api.Console.Services;

/// <summary>
/// Example of how to fetch entities from Bullhorn
/// </summary>
public class PlayGroundService
{
    private readonly ILogger<PlayGroundService> _logger;
    private readonly IBullhornClient _bullhornApi;

    public PlayGroundService(ILogger<PlayGroundService> logger, IBullhornClient bullhornApi)
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
            await QueryExampleAsync(default);
            //await GetCountriesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Entities fetch failed");

            throw;
        }
    }

    private async Task QueryExampleAsync(CancellationToken token)
    {
        var result = await _bullhornApi.QueryAsync<DepartmentDto>($"{EntityType.Department}?fields=id,name&where=id>0", token);

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
