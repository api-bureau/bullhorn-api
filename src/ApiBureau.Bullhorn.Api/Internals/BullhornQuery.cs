namespace ApiBureau.Bullhorn.Api.Internals;

internal static class BullhornQuery
{
    internal static string QuoteAny(IEnumerable<string> values)
        => string.Join(" OR ", values.Select(value => $"\"{value}\""));
}