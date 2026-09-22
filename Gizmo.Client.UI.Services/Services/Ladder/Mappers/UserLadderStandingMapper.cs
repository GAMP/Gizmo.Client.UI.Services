using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

internal static class UserLadderStandingMapper
{
    internal static UserLadderStanding Map(LadderStandingModel source) => new()
    {
        Mode          = LadderEnumMapper.Map(source.Mode),
        State         = LadderEnumMapper.Map(source.State),
        IsFrozen      = source.IsFrozen,
        PeriodKind    = LadderEnumMapper.Map(source.PeriodKind),
        PeriodStart   = source.PeriodStart,
        PeriodEnd     = source.PeriodEnd,
        CurrentRank   = source.CurrentRank,
        ProjectedRank = source.ProjectedRank,
        Score         = source.Score,
        LastScore     = source.LastScore,
        Levels        = source.Levels.Select(Map).ToList(),
        Achievements  = source.Achievements.Select(Map).ToList(),
        Transitions   = source.Transitions.Select(Map).ToList(),
    };

    internal static UserLadderLevel Map(LadderStandingLevelModel source) => new()
    {
        Rank         = source.Rank,
        Name         = source.Name,
        Description  = source.Description,
        Threshold    = source.Threshold,
        Requirements = source.Requirements?.Select(Map).ToList(),
        Progress     = source.Progress,
        MetCount     = source.MetCount,
        IsSatisfied  = source.IsSatisfied,
    };

    internal static UserLadderRequirement Map(LadderStandingRequirementModel source) => new()
    {
        AchievementId = source.AchievementId,
        RequiredCount = source.RequiredCount,
    };

    internal static UserLadderAchievement Map(LadderStandingAchievementModel source) => new()
    {
        AchievementId  = source.AchievementId,
        Name           = source.Name,
        CompletedCount = source.CompletedCount,
        IsHidden       = source.IsHidden,
    };

    internal static UserLadderTransition Map(LadderStandingTransitionModel source) => new()
    {
        Time     = source.Time,
        FromRank = source.FromRank,
        ToRank   = source.ToRank,
        Trigger  = LadderEnumMapper.Map(source.Trigger),
    };

    internal static UserLadderTransition Map(UserAchievementLadderEventModel source) => new()
    {
        Time     = source.CreatedTime,
        FromRank = source.FromRank,
        ToRank   = source.ToRank,
        Trigger  = LadderEnumMapper.Map(source.Trigger),
    };
}
