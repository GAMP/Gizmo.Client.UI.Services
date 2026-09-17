using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

internal static class LadderEnumMapper
{
    internal static LadderMode Map(AchievementLadderMode source) => source switch
    {
        AchievementLadderMode.Points       => LadderMode.Points,
        AchievementLadderMode.Requirements => LadderMode.Requirements,
        _                                  => LadderMode.Unknown,
    };

    internal static LadderStandingState? Map(Gizmo.Web.Api.Models.LadderStandingState? source) => source switch
    {
        null                                              => null,
        Gizmo.Web.Api.Models.LadderStandingState.Earning  => LadderStandingState.Earning,
        Gizmo.Web.Api.Models.LadderStandingState.Secured  => LadderStandingState.Secured,
        Gizmo.Web.Api.Models.LadderStandingState.Awaiting => LadderStandingState.Awaiting,
        _                                                 => LadderStandingState.Unknown,
    };

    internal static LadderPeriod Map(CalendarPeriod source) => source switch
    {
        CalendarPeriod.Day     => LadderPeriod.Day,
        CalendarPeriod.Week    => LadderPeriod.Week,
        CalendarPeriod.Month   => LadderPeriod.Month,
        CalendarPeriod.Quarter => LadderPeriod.Quarter,
        CalendarPeriod.Year    => LadderPeriod.Year,
        _                      => LadderPeriod.Unknown,
    };

    internal static LadderTransitionTrigger Map(AchievementLadderEventTrigger source) => source switch
    {
        AchievementLadderEventTrigger.Live     => LadderTransitionTrigger.Live,
        AchievementLadderEventTrigger.Settle   => LadderTransitionTrigger.Settle,
        AchievementLadderEventTrigger.Operator => LadderTransitionTrigger.Operator,
        _                                      => LadderTransitionTrigger.Unknown,
    };
}
