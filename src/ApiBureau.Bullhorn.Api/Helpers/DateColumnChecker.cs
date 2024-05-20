namespace ApiBureau.Bullhorn.Api.Helpers;

public static class DateColumnChecker
{
    private static readonly List<string> _predefinedColumns =
    [
        "dateAdded", "dateAvailable", "dateBegin", "dateClosed", "dateEnd", "dateLastModified", "dateLastComment", "dateLastVisit", "customDate1", "customDate2", "customDate3", "userDateAdded", "dateLastPublished"
    ];

    public static bool ContainsDateColumn(string columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName))
        {
            throw new ArgumentException("Column name cannot be null or empty.", nameof(columnName));
        }

        return _predefinedColumns.Contains(columnName);
    }

    public static DateTime? ParseValueAsDateTime(string columnName, string value)
    {
        if (long.TryParse(value, out long parsedValue))
        {
            return parsedValue.ToUtcDateTime();
        }

        return null;
    }
}