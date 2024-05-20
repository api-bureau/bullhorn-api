using ApiBureau.Bullhorn.Api.Console.Dtos;
using ApiBureau.Bullhorn.Api.Http;
using ApiBureau.Bullhorn.Api.Interfaces;

namespace ApiBureau.Bullhorn.Api.Console.Services;

/// <summary>
/// Example of how to update a field in Bullhorn.
/// </summary>
public class UpdateFieldService
{
    private readonly ILogger<UpdateFieldService> _logger;
    private readonly IBullhornClient _bullhornApi;

    public UpdateFieldService(ILogger<UpdateFieldService> logger, IBullhornClient bullhornApi)
    {
        _logger = logger;
        _bullhornApi = bullhornApi;
    }

    public async Task UpdateAsync()
    {
        try
        {
            await _bullhornApi.CheckConnectionAsync();

            await UpdatePlacementFieldAsync(6911);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Placement field update failed");

            throw;
        }
    }

    // Important! Make sure you update only a field you want to update. Do not use Dtos with multiple fields which are not going to be updated because Bullhorn entity will be updated with defaults.
    private Task<Result<ChangeResponse>> UpdatePlacementFieldAsync(int placementId)
        => _bullhornApi.Placement.UpdateAsync(placementId, new PlacementUpdateDto { CustomText8 = "New" });
}