using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;

namespace ApiBureau.Bullhorn.Api.Http;

internal sealed class BullhornSessionManager
{
    private readonly HttpClient _client;
    private readonly ApiSession _session;
    private readonly ILogger _logger;
    private readonly TimeSpan _verificationInterval;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private Session? _current;
    private DateTimeOffset _verifiedAt;
    private DateTimeOffset _retryAfter;

    internal BullhornSessionManager(HttpClient client, ApiSession session, ILogger logger, TimeSpan verificationInterval)
    {
        _client = client;
        _session = session;
        _logger = logger;

        if (verificationInterval < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(verificationInterval));

        _verificationInterval = verificationInterval;
    }

    internal sealed record Session(string Token, string RestUrl);

    internal async Task<Session> EnsureAsync(CancellationToken token, bool verify = false, IProgress<string>? progress = null)
    {
        await _gate.WaitAsync(token);

        try
        {
            ThrowIfCoolingDown();

            if (_current is null)
            {
                await RecoverCoreAsync(false, progress, token);
            }
            else if (verify || DateTimeOffset.UtcNow - _verifiedAt >= _verificationInterval)
            {
                if (!await PingAsync(_current, token))
                    await RecoverCoreAsync(true, progress, token);
                else
                    _verifiedAt = DateTimeOffset.UtcNow;
            }

            progress?.Report(verify ? "Bullhorn connection verified." : "Using cached Bullhorn session.");

            return _current!;
        }
        finally { _gate.Release(); }
    }

    internal async Task<Session> RecoverAsync(Session rejected, CancellationToken token)
    {
        await _gate.WaitAsync(token);
        try
        {
            // A concurrent request may already have replaced the rejected session.
            if (_current is not null && !ReferenceEquals(_current, rejected)) return _current;

            ThrowIfCoolingDown();

            await RecoverCoreAsync(true, null, token);

            return _current!;
        }
        finally { _gate.Release(); }
    }

    internal async Task ReconnectAsync(IProgress<string>? progress, CancellationToken token)
    {
        await _gate.WaitAsync(token);

        try { await RecoverCoreAsync(false, progress, token); }
        finally { _gate.Release(); }
    }

    internal async Task InvalidateForTestingAsync(CancellationToken token)
    {
        await EnsureAsync(token);

        await _gate.WaitAsync(token);

        try
        {
            // Keep the recent verification time so a real request encounters the bad token.
            if (_current is null) throw new InvalidOperationException("Bullhorn has no session to invalidate.");
            _current = _current with { Token = "invalid-test-" + Guid.NewGuid().ToString("N") };
            _verifiedAt = DateTimeOffset.UtcNow;
            _logger.LogWarning("Bullhorn cached REST token invalidated for recovery testing.");
        }
        finally { _gate.Release(); }
    }

    private async Task RecoverCoreAsync(bool refresh, IProgress<string>? progress, CancellationToken token)
    {
        _current = null;

        try
        {
            if (refresh)
            {
                progress?.Report("Refreshing Bullhorn session.");

                try { await _session.RefreshTokenAsync(token); }
                catch (InvalidOperationException)
                {
                    // Rejected or unavailable refresh credentials require full authorization.
                    _logger.LogWarning("Bullhorn refresh rejected; starting full authorization.");
                    await _session.ConnectAsync(progress, token);
                }
            }
            else
            {
                await _session.ConnectAsync(progress, token);
            }

            var login = _session.LoginResponse!;
            var candidate = new Session(login.BhRestToken!, login.RestUrl!);

            if (!await PingAsync(candidate, token))
                throw new HttpRequestException("Bullhorn rejected the newly created session.", null, HttpStatusCode.Unauthorized);

            _current = candidate;
            _verifiedAt = DateTimeOffset.UtcNow;
            _retryAfter = default;
            _logger.LogInformation("Bullhorn session recovered and verified.");

            progress?.Report("Bullhorn connection verified.");
        }
        catch
        {
            // Avoid a queue of waiting requests repeatedly logging in during an outage.
            _retryAfter = DateTimeOffset.UtcNow.AddSeconds(5);

            throw;
        }
    }

    internal async Task RejectAsync(Session rejected, CancellationToken token)
    {
        await _gate.WaitAsync(token);

        try
        {
            if (!ReferenceEquals(_current, rejected)) return;

            _current = null;
            _retryAfter = DateTimeOffset.UtcNow.AddSeconds(5);
        }
        finally { _gate.Release(); }
    }

    private void ThrowIfCoolingDown()
    {
        if (DateTimeOffset.UtcNow < _retryAfter)
            throw new HttpRequestException("Bullhorn connection recovery failed recently. Try again in a few seconds.");
    }

    private async Task<bool> PingAsync(Session session, CancellationToken token)
    {
        using var request = CreateRequest(session, HttpMethod.Get, "ping");
        using var response = await _client.SendAsync(request, token);

        if (await IsSessionRejectedAsync(response, token)) return false;

        response.EnsureSuccessStatusCode();

        var ping = await response.Content.ReadFromJsonAsync<PingResponse>(cancellationToken: token)
            ?? throw new HttpRequestException("Bullhorn returned an empty ping response.");

        return ping.Valid;
    }

    internal static HttpRequestMessage CreateRequest(Session session, HttpMethod method, string path)
    {
        var request = new HttpRequestMessage(method, session.RestUrl.TrimEnd('/') + "/" + path.TrimStart('/'));

        request.Headers.TryAddWithoutValidation("BhRestToken", session.Token);

        return request;
    }

    internal static async Task<bool> IsSessionRejectedAsync(HttpResponseMessage response, CancellationToken token)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized) return true;

        if (response.StatusCode is not (HttpStatusCode.BadRequest or HttpStatusCode.Forbidden)) return false;

        var body = await response.Content.ReadAsStringAsync(token);

        return body.Contains("Bad BhRestToken", StringComparison.OrdinalIgnoreCase)
            || body.Contains("Invalid BhRestToken", StringComparison.OrdinalIgnoreCase)
            || body.Contains("Expired BhRestToken", StringComparison.OrdinalIgnoreCase);
    }
}