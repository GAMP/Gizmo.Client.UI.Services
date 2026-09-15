using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

internal static class AchievementEnumMapper
{
    internal static AchievementState Map(UserAchievementState source) => source switch
    {
        UserAchievementState.Active       => AchievementState.Active,
        UserAchievementState.Earned       => AchievementState.Earned,
        UserAchievementState.Paused       => AchievementState.Paused,
        UserAchievementState.Archived     => AchievementState.Archived,
        UserAchievementState.Unmeasurable => AchievementState.Unmeasurable,
        _                                 => AchievementState.Unknown,
    };

    internal static AchievementSignalUnit? Map(SignalUnit? source) => source switch
    {
        SignalUnit.Count    => AchievementSignalUnit.Count,
        SignalUnit.Currency => AchievementSignalUnit.Currency,
        SignalUnit.Duration => AchievementSignalUnit.Duration,
        SignalUnit.Points   => AchievementSignalUnit.Points,
        SignalUnit.Days     => AchievementSignalUnit.Days,
        _                   => null,
    };

    internal static AchievementPeriod Map(CalendarPeriod source) => source switch
    {
        CalendarPeriod.Day     => AchievementPeriod.Day,
        CalendarPeriod.Week    => AchievementPeriod.Week,
        CalendarPeriod.Month   => AchievementPeriod.Month,
        CalendarPeriod.Quarter => AchievementPeriod.Quarter,
        _                      => AchievementPeriod.Year,
    };
}
