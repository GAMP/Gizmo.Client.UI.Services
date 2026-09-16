using System.Globalization;

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
}
