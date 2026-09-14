using System.Net;
using System.Text;
using System.Text.Json;
using ApiBureau.Bullhorn.Api.Core;
using ApiBureau.Bullhorn.Api.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace ApiBureau.Bullhorn.Api.Tests;

public sealed class ConnectionRecoveryTests
{
    [Fact]
    public async Task InvalidTokenRecoversAndRetriesSearchWithNewRestUrl()
    {
        using var server = new Server();
        var client = server.CreateClient();
        Assert.True(await client.CheckConnectionAsync());
        await client.Advanced.InvalidateSessionForTestingAsync(TestContext.Current.CancellationToken);
        var items = await client.Candidates.GetAddedSinceAsync(DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Empty(items);
        Assert.Equal(2, server.Logins);
        Assert.Equal(1, server.Refreshes);
        Assert.Equal(2, server.Reads);
        Assert.Equal("/rest/2/search/Candidate", server.LastReadPath);
    }

    [Fact]
    public async Task RejectedRefreshFallsBackToFullAuthorization()
    {
        using var server = new Server { RejectRefresh = true };
        var client = server.CreateClient();
        await client.CheckConnectionAsync();
        await client.Advanced.InvalidateSessionForTestingAsync(TestContext.Current.CancellationToken);
        await client.Candidates.GetAddedSinceAsync(DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(2, server.Authorizations);
        Assert.Equal(1, server.Refreshes);
        Assert.Equal(2, server.Logins);
    }

    [Fact]
    public async Task ConcurrentRejectedRequestsShareRecovery()
    {
        using var server = new Server { RejectedRequestCount = 8 };
        var client = server.CreateClient();
        await client.CheckConnectionAsync();
        await client.Advanced.InvalidateSessionForTestingAsync(TestContext.Current.CancellationToken);
        await Task.WhenAll(Enumerable.Range(0, 8)
            .Select(_ => client.Candidates.GetAddedSinceAsync(DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken)));
        Assert.Equal(1, server.Refreshes);
        Assert.Equal(2, server.Logins);
    }

    [Fact]
    public async Task ManualReconnectAlwaysRunsFullAuthorization()
    {
        using var server = new Server();
        var client = server.CreateClient();
        await client.CheckConnectionAsync();
        await client.Advanced.ReconnectAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(2, server.Authorizations);
        Assert.Equal(0, server.Refreshes);
    }

    [Fact]
    public async Task VerifyDetectsServerInvalidationDespiteCachedExpiry()
    {
        using var server = new Server();
        var client = server.CreateClient();
        await client.CheckConnectionAsync();
        server.RejectCurrentSession = true;
        await client.Advanced.VerifyConnectionAsync(TestContext.Current.CancellationToken);
        Assert.Equal(1, server.Refreshes);
    }

    [Theory]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task NonAuthenticationErrorsAreVisibleAndDoNotReconnect(HttpStatusCode status)
    {
        using var server = new Server { ReadStatus = status };
        var client = server.CreateClient();
        var error = await Assert.ThrowsAsync<HttpRequestException>(
            () => client.Candidates.GetAddedSinceAsync(DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(status, error.StatusCode);
        Assert.Contains("permission or server failure", error.Message);
        Assert.Equal(1, server.Logins);
        Assert.Equal(1, server.Reads);
    }

    [Fact]
    public async Task PersistentRejectionStopsAfterOneReplay()
    {
        using var server = new Server { ReadStatus = HttpStatusCode.Unauthorized };
        var client = server.CreateClient();
        await Assert.ThrowsAsync<HttpRequestException>(
            () => client.Candidates.GetAddedSinceAsync(DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(2, server.Reads);
        Assert.Equal(2, server.Logins);
        Assert.False(await client.CheckConnectionAsync());
    }

    [Fact]
    public async Task WriteRepairsSessionWithoutReplayingMutation()
    {
        using var server = new Server();
        var client = server.CreateClient();
        await client.CheckConnectionAsync();
        await client.Advanced.InvalidateSessionForTestingAsync(TestContext.Current.CancellationToken);
        var result = await client.Appointments.UpdateAsync(1, new { subject = "test" });
        Assert.True(result.IsFailure);
        Assert.Equal(1, server.Writes);
        Assert.Equal(1, server.Refreshes);
        await client.Candidates.GetAddedSinceAsync(DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(1, server.Reads);
    }

    [Fact]
    public async Task TimeoutIsSurfacedWithoutReconnect()
    {
        using var server = new Server { TimeoutReads = true };
        var client = server.CreateClient();
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => client.Candidates.GetAddedSinceAsync(DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(1, server.Logins);
        Assert.Equal(1, server.Reads);
    }

    [Fact]
    public async Task CancellationDoesNotStartAuthorization()
    {
        using var server = new Server();
        var client = server.CreateClient();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.Advanced.ReconnectAsync(cancellationToken: cancellation.Token));
        Assert.Equal(0, server.Authorizations);
    }

    [Fact]
    public async Task LaterPageFailureDoesNotReturnPartialResults()
    {
        using var server = new Server { FailSecondPage = true };
        var client = server.CreateClient();
        await Assert.ThrowsAsync<HttpRequestException>(
            () => client.Candidates.GetAddedSinceAsync(DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(2, server.Reads);
    }

    [Fact]
    public async Task InvalidSuccessPayloadDoesNotBecomeEmptySearch()
    {
        using var server = new Server { InvalidPage = true };
        var client = server.CreateClient();
        await Assert.ThrowsAsync<HttpRequestException>(() => client.Candidates.GetAddedSinceAsync(
            DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task QueryFailureDoesNotBecomeEmptyResult()
    {
        using var server = new Server { ReadStatus = HttpStatusCode.InternalServerError };
        var client = server.CreateClient();
        await Assert.ThrowsAsync<HttpRequestException>(() => client.Appointments.GetAddedSinceAsync(
            0, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task WriteTimeoutIsNotReportedAsSuccess()
    {
        using var server = new Server { TimeoutWrites = true };
        var client = server.CreateClient();
        await Assert.ThrowsAsync<TaskCanceledException>(() => client.Appointments.UpdateAsync(1, new { subject = "test" }));
        Assert.Equal(1, server.Writes);
        Assert.Equal(1, server.Logins);
    }

    [Fact]
    public async Task ConcurrentInitialRequestsShareLogin()
    {
        using var server = new Server();
        var client = server.CreateClient();
        await Task.WhenAll(Enumerable.Range(0, 8).Select(_ =>
            client.Candidates.GetAddedSinceAsync(DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken)));
        Assert.Equal(1, server.Authorizations);
    }

    [Fact]
    public void PingUsesUtcEpochAndSafetyMargin()
    {
        Assert.True(new PingResponse { SessionExpires = DateTimeOffset.UtcNow.AddMinutes(2).ToUnixTimeMilliseconds() }.Valid);
        Assert.False(new PingResponse { SessionExpires = DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeMilliseconds() }.Valid);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    public async Task RejectedLoginDoesNotRetryOrRestartAuthorization(HttpStatusCode status)
    {
        using var server = new Server { LoginStatus = status };
        var client = server.CreateClient();
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            client.Advanced.ReconnectAsync(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(1, server.Authorizations);
        Assert.Equal(1, server.TokenExchanges);
        Assert.Equal(1, server.Logins);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadGateway)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    [InlineData(HttpStatusCode.GatewayTimeout)]
    public async Task TransientLoginRetriesOnlyLogin(HttpStatusCode status)
    {
        using var server = new Server { LoginStatus = status, FailLoginOnce = true };
        var client = server.CreateClient();
        await client.Advanced.ReconnectAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(1, server.Authorizations);
        Assert.Equal(1, server.TokenExchanges);
        Assert.Equal(2, server.Logins);
    }

    [Fact]
    public async Task PersistentTransientLoginStopsAfterTwoAttempts()
    {
        using var server = new Server { LoginStatus = HttpStatusCode.ServiceUnavailable };
        var client = server.CreateClient();
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            client.Advanced.ReconnectAsync(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(1, server.TokenExchanges);
        Assert.Equal(2, server.Logins);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, "invalid_client")]
    [InlineData(HttpStatusCode.ServiceUnavailable, "server_error")]
    public async Task OtherRefreshFailuresDoNotRepeatTheGrantOrFallBack(HttpStatusCode status, string code)
    {
        using var server = new Server();
        var client = server.CreateClient();
        await client.CheckConnectionAsync();
        server.RefreshStatus = status;
        server.RefreshError = code;
        await client.Advanced.InvalidateSessionForTestingAsync(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HttpRequestException>(() => client.Candidates.GetAddedSinceAsync(
            DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(1, server.Refreshes);
        Assert.Equal(1, server.Authorizations);
        Assert.Equal(1, server.Logins);
    }

    [Fact]
    public async Task ConsecutiveRecoveriesUseTheLatestRotatedRefreshToken()
    {
        using var server = new Server();
        var client = server.CreateClient();
        await client.CheckConnectionAsync();
        for (var i = 0; i < 2; i++)
        {
            await client.Advanced.InvalidateSessionForTestingAsync(TestContext.Current.CancellationToken);
            await client.Candidates.GetAddedSinceAsync(DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken);
        }
        Assert.Equal(2, server.Refreshes);
        Assert.Equal(1, server.Authorizations);
    }

    [Fact]
    public async Task LoginTimeoutDoesNotReplayAuthorizationOrLogin()
    {
        using var server = new Server { TimeoutLogin = true };
        var client = server.CreateClient();
        await Assert.ThrowsAsync<TaskCanceledException>(() =>
            client.Advanced.ReconnectAsync(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(1, server.TokenExchanges);
        Assert.Equal(1, server.Logins);
    }

    [Fact]
    public async Task RotatedRefreshTokenSurvivesAFailedRestLogin()
    {
        using var server = new Server();
        var client = server.CreateClient();
        await client.CheckConnectionAsync();
        await client.Advanced.InvalidateSessionForTestingAsync(TestContext.Current.CancellationToken);
        server.LoginStatus = HttpStatusCode.BadRequest;
        await Assert.ThrowsAsync<HttpRequestException>(() => client.Candidates.GetAddedSinceAsync(
            DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken));

        // Wait out the public recovery cooldown without modifying private session state.
        await Task.Delay(TimeSpan.FromSeconds(5.1), TestContext.Current.CancellationToken);
        server.LoginStatus = HttpStatusCode.OK;
        await client.Candidates.GetAddedSinceAsync(DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(1, server.Authorizations);
        Assert.Equal(2, server.Refreshes);
        Assert.Equal(3, server.TokenExchanges);
    }

    private sealed class Server : HttpMessageHandler
    {
        private HttpClient? _http;
        private int _rejected;
        private readonly TaskCompletionSource _allRejected = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public int Logins;
        public int TokenExchanges;
        public HttpStatusCode LoginStatus = HttpStatusCode.OK;
        public HttpStatusCode RefreshStatus = HttpStatusCode.OK;
        public string RefreshError = "server_error";
        public bool FailLoginOnce;
        public bool TimeoutLogin;
        public int Authorizations;
        public int Refreshes;
        public int Reads;
        public int Writes;
        public int RejectedRequestCount = 1;
        public bool RejectRefresh;
        public bool RejectCurrentSession;
        public bool TimeoutReads;
        public bool TimeoutWrites;
        public bool InvalidPage;
        public bool FailSecondPage;
        public HttpStatusCode ReadStatus = HttpStatusCode.OK;
        public string? LastReadPath;

        public BullhornClient CreateClient()
        {
            _http = new HttpClient(this, disposeHandler: false);
            var settings = new BullhornSettings
            {
                AuthorizeUrl = "https://example.test/authorize",
                TokenUrl = "https://example.test/token",
                LoginUrl = "https://example.test/login",
                AuthorizationParameter = "code",
                ClientId = "client", Secret = "secret", UserName = "user", Password = "password"
            };
            return new BullhornClient(new BullhornHttpClient(_http, Options.Create(settings),
                NullLogger<BullhornHttpClient>.Instance));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            var path = request.RequestUri!.AbsolutePath;
            if (path == "/authorize")
            {
                Interlocked.Increment(ref Authorizations);
                var response = Json(new { });
                response.Headers.Location = new Uri("https://example.test/callback?code=code");
                return response;
            }
            if (path == "/token")
            {
                var body = await request.Content!.ReadAsStringAsync(token);
                if (body.Contains("grant_type=refresh_token"))
                {
                    Interlocked.Increment(ref Refreshes);
                    if (RefreshStatus != HttpStatusCode.OK) return Json(new { error = RefreshError }, RefreshStatus);
                    if (RejectRefresh || !body.Contains("refresh_token=refresh-" + TokenExchanges))
                        return Json(new { error = "invalid_grant" }, HttpStatusCode.BadRequest);
                }
                var generation = Interlocked.Increment(ref TokenExchanges);
                return Json(new { access_token = "access-" + generation, refresh_token = "refresh-" + generation });
            }
            if (path == "/login")
            {
                var version = Interlocked.Increment(ref Logins);
                if (TimeoutLogin) throw new TaskCanceledException("Simulated login timeout");
                if (LoginStatus != HttpStatusCode.OK && (!FailLoginOnce || version == 1))
                    return Json(new { errorMessage = "Login failed" }, LoginStatus);
                RejectCurrentSession = false;
                return Json(new { BhRestToken = "token-" + version, restUrl = "https://example.test/rest/" + version + "/" });
            }
            var rejected = RejectCurrentSession ||
                request.Headers.GetValues("BhRestToken").Single() != "token-" + Logins;
            if (path.EndsWith("/ping"))
                return rejected ? Json(new { errorMessage = "Bad BhRestToken" }, HttpStatusCode.BadRequest)
                    : Json(new { sessionExpires = DateTimeOffset.UtcNow.AddHours(4).ToUnixTimeMilliseconds() });
            if (request.Method != HttpMethod.Get)
            {
                Interlocked.Increment(ref Writes);
                if (TimeoutWrites) throw new TaskCanceledException("Simulated write timeout");
                return rejected ? Json(new { errorMessage = "Bad BhRestToken" }, HttpStatusCode.BadRequest)
                    : Json(new { changedEntityId = 1 });
            }
            var read = Interlocked.Increment(ref Reads);
            LastReadPath = path;
            if (rejected)
            {
                if (Interlocked.Increment(ref _rejected) >= RejectedRequestCount) _allRejected.TrySetResult();
                await _allRejected.Task.WaitAsync(TimeSpan.FromSeconds(5), token);
                return Json(new { errorMessage = "Bad BhRestToken" }, HttpStatusCode.BadRequest);
            }
            if (InvalidPage) return Json(new { errorMessage = "unexpected payload" });
            if (TimeoutReads) throw new TaskCanceledException("Simulated timeout");
            if (ReadStatus != HttpStatusCode.OK || (FailSecondPage && read > 1))
                return Json(new { errorMessage = "permission or server failure" },
                    FailSecondPage ? HttpStatusCode.InternalServerError : ReadStatus);
            if (FailSecondPage) return Json(new { data = new[] { new { id = 1 } }, count = 1, total = 2 });
            return Json(new { data = Array.Empty<object>(), count = 0, total = 0 });
        }

        private static HttpResponseMessage Json(object value, HttpStatusCode status = HttpStatusCode.OK)
            => new(status) { Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json") };

        protected override void Dispose(bool disposing)
        {
            if (disposing) _http?.Dispose();
            base.Dispose(disposing);
        }
    }
}
