using System;
using System.Globalization;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Display formatting for achievement values by signal unit.
    /// </summary>
    public static class AchievementValueFormat
    {
        public static string Value(decimal value, AchievementSignalUnit? unit) => unit switch
        {
            AchievementSignalUnit.Duration => Duration(value),
            AchievementSignalUnit.Currency => value.ToString("C2", CultureInfo.CurrentCulture),
            _ => Trim(value),
        };

        public static string Pair(decimal current, decimal target, AchievementSignalUnit? unit) =>
            $"{Value(current, unit)} / {Value(target, unit)}";

        /// <summary>Seconds → "4.5 h" / "6 h" / "45 min".</summary>
        public static string Duration(decimal seconds)
        {
            var hours = seconds / 3600m;
            return hours >= 1m
                ? Trim(Math.Round(hours, 1)) + " h"
                : Trim(Math.Round(seconds / 60m, 0)) + " min";
        }

        /// <summary>Whole numbers without decimals, fractions with up to two significant decimals ("6", "4.5", "0.75").</summary>
        public static string Trim(decimal value) =>
            value.ToString("#,0.##", CultureInfo.CurrentCulture);

        public static string Percent(decimal? progress) =>
            Math.Clamp(progress ?? 0m, 0m, 100m).ToString("0.##", CultureInfo.InvariantCulture) + "%";
    }
}
