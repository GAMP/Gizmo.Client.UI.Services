using System.Globalization;
using System.Text.RegularExpressions;

namespace Gizmo.Client.UI.Services;

/// <summary>
/// Date display helpers shared by the profile tab view services.
/// </summary>
internal static class ProfileDateFormat
{
    /// <summary>
    /// Culture-ordered month/day with an abbreviated month, in local time: "Oct 1" (en-US), "1 окт." (ru-RU).
    /// </summary>
    internal static string MonthDay(DateTime utc)
    {
        var culture = CultureInfo.CurrentCulture;
        var pattern = culture.DateTimeFormat.MonthDayPattern.Replace("MMMM", "MMM");
        return utc.ToLocalTime().ToString(pattern, culture);
    }

    /// <summary>
    /// Culture-ordered month/day/year with an abbreviated month and no weekday, in local time:
    /// "Sep 2, 2026" (en-US), "2 сент. 2026 г." (ru-RU) — the culture's long date pattern with the
    /// weekday token, its adjacent separators and a quoted literal that follows it (da-DK 'den') removed.
    /// </summary>
    internal static string MonthDayYear(DateTime utc)
    {
        var culture = CultureInfo.CurrentCulture;
        var pattern = Regex.Replace(culture.DateTimeFormat.LongDatePattern, @"(,?\s*dddd(?:\s*'[^']*')?,?\s*)", " ")
            .Replace("MMMM", "MMM")
            .Trim();
        return utc.ToLocalTime().ToString(pattern, culture);
    }
}
