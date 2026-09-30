using System.Globalization;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services;

/// <summary>
/// Standing-card values (period, score, progress, segments, banner, requirements, level info)
/// shared by the ladder tab and the top-bar popover, so the two surfaces never disagree.
/// </summary>
internal static class UserLadderCardText
{
    internal readonly record struct UserLadderCardProgress(bool Show, decimal Percent, bool IsFull, bool GoalIsReach, string GoalText);

    internal readonly record struct UserLadderCardSegments(bool Show, int Count, int Lit, string LabelText, string CountText, string UnitText);

    internal readonly record struct UserLadderCardRequirements(bool Show, string LabelText, IEnumerable<UserLadderRequirementViewState> Rows);

    internal static string PeriodText(ILocalizationService loc, LadderPeriod period) => period switch
    {
        LadderPeriod.Day     => loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_DAY)),
        LadderPeriod.Week    => loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_WEEK)),
        LadderPeriod.Month   => loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_MONTH)),
        LadderPeriod.Quarter => loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_QUARTER)),
        LadderPeriod.Year    => loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_YEAR)),
        _                    => string.Empty,
    };

    internal static string ScoreUnitText(ILocalizationService loc, LadderPeriod period) => period switch
    {
        LadderPeriod.Day     => loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_SCORE_UNIT_DAY)),
        LadderPeriod.Week    => loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_SCORE_UNIT_WEEK)),
        LadderPeriod.Month   => loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_SCORE_UNIT_MONTH)),
        LadderPeriod.Quarter => loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_SCORE_UNIT_QUARTER)),
        LadderPeriod.Year    => loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_SCORE_UNIT_YEAR)),
        _                    => string.Empty,
    };

    internal static string DaysLeftText(ILocalizationService loc, UserLadderStanding s)
    {
        int daysLeft = Math.Max(0, (int)Math.Ceiling((s.PeriodEnd - DateTime.UtcNow).TotalDays));
        return daysLeft == 0
            ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_ENDS_TODAY))
            : loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_DAYS_LEFT), daysLeft);
    }

    internal static bool ShowScore(UserLadderStanding s) => s.Mode == LadderMode.Points && s.Score is not null;

    internal static string ScoreText(UserLadderStanding s) => s.Score is decimal score ? AchievementValueFormat.Trim(decimal.Floor(score)) : string.Empty;

    internal static UserLadderCardProgress Progress(ILocalizationService loc, UserLadderStanding s)
    {
        var current = s.CurrentLevel();
        var next = s.NextLevel();
        bool secured = s.IsSecured();
        bool atEntry = s.IsAtEntry();

        int? keep = (atEntry || s.IsOnlyLevel()) ? null : current?.Threshold;
        if (keep is <= 0)
            keep = null;
        int? reach = next?.Threshold;
        if (reach is <= 0)
            reach = null;

        int? scale;
        bool goalIsReach;
        if (secured || atEntry)
        {
            goalIsReach = reach is not null;
            scale = reach ?? keep;
        }
        else
        {
            goalIsReach = keep is null && reach is not null;
            scale = keep ?? reach;
        }

        bool show = ShowScore(s) && scale is not null;
        decimal percent = show
            ? Math.Clamp(s.Score!.Value / scale!.Value * 100m, 0m, 100m)
            : 0m;
        string goalText = scale is null
            ? string.Empty
            : goalIsReach
                ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_GOAL_REACH), AchievementValueFormat.Trim(scale.Value), next!.Name)
                : loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_GOAL_RETAIN), AchievementValueFormat.Trim(scale.Value));

        return new UserLadderCardProgress(show, percent, show && percent >= 100m, goalIsReach, goalText);
    }

    internal static UserLadderCardSegments Segments(ILocalizationService loc, UserLadderStanding s)
    {
        var current = s.CurrentLevel();
        var target = s.NextLevel();
        int? total = target?.Requirements?.Count;
        int? met = target?.MetCount;
        bool hasTarget = s.Mode == LadderMode.Requirements && current is not null && target is not null && total is > 0;

        if (!(hasTarget && met is not null))
            return new UserLadderCardSegments(false, 0, 0, string.Empty, string.Empty, string.Empty);

        int count = total!.Value;
        int lit = Math.Clamp(met!.Value, 0, count);

        return new UserLadderCardSegments(
            true,
            count,
            lit,
            loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PROGRESS_TO), target!.Name),
            loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_MET_COUNT), lit, count),
            loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_ACHIEVEMENTS_UNIT)));
    }

    internal static string BannerTitleText(ILocalizationService loc, UserLadderStanding s)
    {
        var current = s.CurrentLevel();
        return current is null
            ? string.Empty
            : loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BANNER_SECURED), current.Name);
    }

    internal static string BannerDetailText(ILocalizationService loc, UserLadderStanding s)
    {
        var next = s.NextLevel();
        string awaiting = UserLadderStatusText.AwaitingStatus(loc, s);
        return awaiting.Length > 0
            ? awaiting
            : next is null
                ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BANNER_TOP))
                : s.Score is decimal sc && next.Threshold is int t
                    ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BANNER_NEXT), AchievementValueFormat.Trim(decimal.Ceiling(Math.Max(0m, t - sc))), next.Name)
                    : string.Empty;
    }

    internal static string ProgressUpdatingText(ILocalizationService loc, UserLadderStanding s) =>
        !s.IsFrozen && s.State is null && s.CurrentLevel() is not null
            ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PROGRESS_UPDATING))
            : string.Empty;

    internal static string FrozenText(ILocalizationService loc, UserLadderStanding s) =>
        s.IsFrozen && s.CurrentLevel() is not null
            ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_FROZEN))
            : string.Empty;

    internal static UserLadderCardRequirements TargetRequirements(ILocalizationService loc, UserLadderStanding s)
    {
        var target = s.NextLevel();
        var requirements = s.Mode == LadderMode.Requirements && s.CurrentLevel() is not null && UserLadderStatusText.IsProgressCollected(s)
            ? target?.Requirements
            : null;

        if (target is null || requirements is not { Count: > 0 })
            return new UserLadderCardRequirements(false, string.Empty, Enumerable.Empty<UserLadderRequirementViewState>());

        var rows = RequirementRows(loc, s, requirements);

        if (rows.Count == 0)
            return new UserLadderCardRequirements(false, string.Empty, Enumerable.Empty<UserLadderRequirementViewState>());

        return new UserLadderCardRequirements(
            true,
            loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_REQUIREMENTS_FOR), target.Name),
            rows);
    }

    internal static List<UserLadderRequirementViewState> RequirementRows(ILocalizationService loc, UserLadderStanding s, IReadOnlyList<UserLadderRequirement> requirements) => requirements
        .Select(requirement => new { requirement, achievement = s.FindAchievement(requirement.AchievementId) })
        .Where(row => row.achievement is not null)
        .Select(row =>
        {
            bool isMet = row.achievement!.CompletedCount >= row.requirement.RequiredCount;

            return new UserLadderRequirementViewState
            {
                AchievementId = row.requirement.AchievementId,
                Name = row.achievement.Name,
                IsMet = isMet,
                IsLink = !isMet && !row.achievement.IsHidden,
                CountText = row.requirement.RequiredCount > 1 && row.achievement.CompletedCount is int done
                    ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_MET_COUNT), Math.Min(done, row.requirement.RequiredCount), row.requirement.RequiredCount)
                    : string.Empty,
                ValueText = !isMet && !row.achievement.IsHidden && row.achievement.CurrentValue is decimal current
                        && row.achievement.TargetValue > 0 && current < row.achievement.TargetValue
                    ? AchievementValueFormat.Pair(Math.Max(current, 0m), row.achievement.TargetValue, row.achievement.Unit, loc)
                    : string.Empty,
            };
        })
        .ToList();

    internal static UserLadderLevelViewState Level(ILocalizationService loc, UserLadderStanding s, int index, int? selectedRank)
    {
        var level = s.Levels[index];
        bool isRequirements = s.Mode == LadderMode.Requirements;
        bool collected = UserLadderStatusText.IsProgressCollected(s);
        int? projectedRank = collected && s.ProjectedRank != s.CurrentRank ? s.ProjectedRank : null;
        var next = s.NextLevel();
        int nextIndex = -1;
        if (next is not null)
        {
            for (int i = 0; i < s.Levels.Count; i++)
            {
                if (s.Levels[i].Rank == next.Rank)
                {
                    nextIndex = i;
                    break;
                }
            }
        }

        var perkTexts = level.Perks.Select(perk => PerkText(loc, perk)).Where(text => text.Length > 0).ToList();
        bool hasDescription = !string.IsNullOrWhiteSpace(level.Description);
        bool isLocked = isRequirements && nextIndex >= 0 && index > nextIndex;
        var requirementRows = isRequirements && collected && level.Requirements is { Count: > 0 } levelRequirements
            ? RequirementRows(loc, s, levelRequirements)
            : new List<UserLadderRequirementViewState>();
        string reachFirstText = s.IsStepwise && isLocked && index > 0
            ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_REACH_FIRST), s.Levels[index - 1].Name)
            : string.Empty;

        return new UserLadderLevelViewState
        {
            Rank = level.Rank,
            Ordinal = index + 1,
            Name = level.Name,
            IsCurrent = level.Rank == s.CurrentRank,
            IsSatisfied = level.IsSatisfied == true,
            IsSelected = level.Rank == selectedRank,
            MetaText = isRequirements
                ? (level.Requirements is { Count: > 0 } requirements && level.MetCount is int metCount
                    ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_MET_COUNT), metCount, requirements.Count)
                    : string.Empty)
                : (level.Threshold is int threshold && threshold > 0
                    ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_LEVEL_POINTS), AchievementValueFormat.Trim(threshold))
                    : string.Empty),
            IsNext = isRequirements && nextIndex >= 0 && index == nextIndex,
            IsLocked = isLocked,
            IsProjected = level.Rank == projectedRank,
            IsProjectedDown = level.Rank == projectedRank && level.Rank < s.CurrentRank,
            EmblemUrl = level.EmblemUrl,
            Description = level.Description?.Trim() ?? string.Empty,
            HasDescription = hasDescription,
            PerkTexts = perkTexts,
            HasPerks = perkTexts.Count > 0,
            Requirements = requirementRows,
            HasRequirements = requirementRows.Count > 0,
            ReachFirstText = reachFirstText,
            HasInfo = hasDescription || perkTexts.Count > 0 || requirementRows.Count > 0 || reachFirstText.Length > 0,
        };
    }

    private static string PerkText(ILocalizationService loc, UserLadderPerk perk)
    {
        if (perk.Kind == LadderPerkKind.WaitingLine)
            return loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERK_QUEUE_PRIORITY), perk.Priority);

        string magnitude = (perk.IsBonus ? "+" : string.Empty)
            + (perk.IsPercentage ? AchievementValueFormat.Trim(perk.Value) + "%" : perk.Value.ToString("C2", CultureInfo.CurrentCulture));

        return string.IsNullOrWhiteSpace(perk.Name)
            ? magnitude
            : loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERK_DISCOUNT), perk.Name.Trim(), magnitude);
    }
}
