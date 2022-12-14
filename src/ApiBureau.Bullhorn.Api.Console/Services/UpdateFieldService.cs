namespace ApiBureau.Bullhorn.Api.Console.Services;

/// <summary>
/// Example of how to update a field in Bullhorn.
/// </summary>
public class UpdateFieldService
{
    private readonly ILogger<UpdateFieldService> _logger;
    private readonly BullhornClient _bullhornApi;

    public UpdateFieldService(ILogger<UpdateFieldService> logger, BullhornClient bullhornApi)
    {
        _logger = logger;
        _bullhornApi = bullhornApi;
    }

    public async Task UpdateAsync()
    {
        try
        {
            await _bullhornApi.CheckConnectionAsync();

            await UpdatePlacmentFieldAsync(6911);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Placement field udpate failed");

            throw;
        }
    }

    // Important! Make sure you update only a field you want to update. Do not use Dtos with multiple fields which are not going to be updated because Bullhorn entity will be updated with defaults.
    private Task UpdatePlacmentFieldAsync(int placementId)
        => _bullhornApi.Placement.UpdateAsync(placementId, new { customText8 = "New" });
}
