using System.Net.Http.Json;
using System.Text.Json;

namespace ApiBureau.Bullhorn.Api.Http;

internal static class BullhornResponseReader
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
    private static readonly JsonSerializerOptions PageOptions = new(JsonSerializerDefaults.Web)
    {
        AllowTrailingCommas = true
    };

    internal static async Task<T> ReadPageAsync<T>(HttpResponseMessage response, CancellationToken token)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await ReadErrorAsync(response, token);

            throw new HttpRequestException(error.Message, null, response.StatusCode);
        }

        using var document = await response.Content.ReadFromJsonAsync<JsonDocument>(PageOptions, token);

        if (document is null || document.RootElement.ValueKind != JsonValueKind.Object ||
            !document.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            throw new HttpRequestException("Bullhorn returned an invalid page: the data array is missing.");

        return document.RootElement.Deserialize<T>(PageOptions)
            ?? throw new HttpRequestException("Bullhorn returned an empty page.");
    }

    internal static async Task<Result<TSuccess, ErrorResponse>> ReadResultAsync<TSuccess>(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<TSuccess, ErrorResponse>(await ReadErrorAsync(response, cancellationToken));
        }

        try
        {
            var success = await response.Content.ReadFromJsonAsync<TSuccess>(Options, cancellationToken);

            return success is null
                ? Result.Failure<TSuccess, ErrorResponse>(ErrorResponse.FromMessage("Response deserialization failed.", response))
                : Result.Success<TSuccess, ErrorResponse>(success);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return Result.Failure<TSuccess, ErrorResponse>(ErrorResponse.FromMessage(exception.Message, response));
        }
    }

    internal static async Task<ErrorResponse> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(Options, cancellationToken)
                ?? ErrorResponse.FromMessage(null, response);

            error.HttpStatusCode = (int)response.StatusCode;
            error.ReasonPhrase = response.ReasonPhrase;
            error.ErrorMessage = string.IsNullOrWhiteSpace(error.ErrorMessage) && string.IsNullOrWhiteSpace(error.ErrorsFormatted)
                ? response.ReasonPhrase ?? "The Bullhorn request failed."
                : error.ErrorMessage;

            return error;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return ErrorResponse.FromMessage(exception.Message, response);
        }
    }
}