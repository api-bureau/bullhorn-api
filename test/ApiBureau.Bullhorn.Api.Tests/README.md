# Bullhorn connection regression tests

Run the xUnit executable runner (no Bullhorn credentials or network calls):

```powershell
dotnet run --project test/ApiBureau.Bullhorn.Api.Tests -c Debug
```

The fake HTTP server exercises authorization, token refresh, REST login, ping,
search pagination, concurrent recovery, cancellation and mutation failures.

In Twenty360, open Developer Tools, then Diagnostics:

1. Use Verify connection to confirm server health.
2. Confirm shared-session invalidation and click Invalidate token.
3. Immediately click Test recovery. This sends a real candidate search; successful
   recovery is recorded in the application log.
4. Repeat invalidation and click Reconnect Bullhorn to test full authorization.

Invalidation changes only the web process's cached token. It does not revoke a
Bullhorn session or alter records. All consumers of that singleton client share
the test token, so a background request may trigger recovery before the button.
Other processes (including automation workers) have their own sessions.

Existing list-returning endpoints now throw on HTTP, deserialization and incomplete
pagination failures. Successful empty pages still return empty lists. Twenty360's
service layer translates these exceptions into its existing Result failures.

Automatic recovery replays reads once. Writes are never replayed automatically;
an authentication rejection repairs the session but returns the original failure.
Timeouts propagate and do not force reauthorization. Session verification defaults
to one minute and can be configured with BullhornSettings.SessionVerificationInterval.
