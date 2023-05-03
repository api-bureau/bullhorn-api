using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ApiBureau.Bullhorn.Api.Helpers;

public static class JsonHelper
{

    public static async Task<T?> DeserializeAsync<T>(this HttpResponseMessage response, ILogger? logger = null)
    {
        var options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = true
        };

        await using var stream = await response.Content.ReadAsStreamAsync();

        try
        {
            return await JsonSerializer.DeserializeAsync<T>(stream, options);
        }
        catch (Exception e)
        {
            logger?.LogError(e, "Deserialize Error at {uri}", response.RequestMessage?.RequestUri);
            throw;
        }
    }
}