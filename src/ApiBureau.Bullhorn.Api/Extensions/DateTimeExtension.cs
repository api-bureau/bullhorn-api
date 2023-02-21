namespace ApiBureau.Bullhorn.Api.Extensions;

public static class DateTimeExtension
{
    public static DateTime ToDateTime(this long timestamp)
        => DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;

    public static long Timestamp(this DateTime datetime)
        => new DateTimeOffset(datetime).ToUnixTimeMilliseconds();
}