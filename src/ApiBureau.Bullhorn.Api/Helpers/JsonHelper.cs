using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ApiBureau.Bullhorn.Api.Helpers;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions _cachedOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static async Task<T?> DeserializeAsync<T>(this HttpResponseMessage response, ILogger? logger = null)
    {
        if (!response.IsSuccessStatusCode)
        {
            logger?.LogWarning("Response deserialization skipped for unsuccessful status {statusCode} at {uri}",
                (int)response.StatusCode, response.RequestMessage?.RequestUri);

            return default;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();

        try
        {
            return await JsonSerializer.DeserializeAsync<T>(stream, _cachedOptions);
        }
        catch (Exception e)
        {
            logger?.LogError(e, "Deserialize Error at {uri}", response.RequestMessage?.RequestUri);
            throw;
        }
    }
}