using System;
using System.Globalization;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Display formatting for achievement values by signal unit.
    /// </summary>
    public static class AchievementValueFormat
    {
        public static string Value(decimal value, AchievementSignalUnit? unit, ILocalizationService localizationService) => unit switch
        {
            AchievementSignalUnit.Duration => Duration(value, localizationService),
            AchievementSignalUnit.Currency => value.ToString("C2", CultureInfo.CurrentCulture),
            _ => Trim(value),
        };

        public static string Pair(decimal current, decimal target, AchievementSignalUnit? unit, ILocalizationService localizationService) =>
            $"{Value(current, unit, localizationService)} / {Value(target, unit, localizationService)}";

        /// <summary>
        /// Seconds → "4.5 h" / "6 h" / "45 min" with the localized short unit names.
        /// </summary>
        public static string Duration(decimal seconds, ILocalizationService localizationService)
        {
            var hours = seconds / 3600m;
            return hours >= 1m
                ? Trim(Math.Round(hours, 1)) + " " + localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_UNIT_HOURS))
                : Trim(Math.Round(seconds / 60m, 0)) + " " + localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_UNIT_MINUTES));
        }

        /// <summary>
        /// Whole numbers without decimals, fractions with up to two significant decimals ("6", "4.5", "0.75").
        /// </summary>
        public static string Trim(decimal value) =>
            value.ToString("#,0.##", CultureInfo.CurrentCulture);

        public static string Percent(decimal? progress) =>
            Math.Clamp(progress ?? 0m, 0m, 100m).ToString("0.##", CultureInfo.InvariantCulture) + "%";
    }
}
