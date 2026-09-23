using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services;

/// <summary>
/// Ladder status/progress texts shared by the ladder tab, the profile-header block and the
/// top-bar block, so the three surfaces never compute them independently.
/// </summary>
internal static class UserLadderStatusText
{
    internal static bool IsProgressCollected(UserLadderStanding s) => !s.IsFrozen && s.State is not null;

    internal static string RequirementsStatus(ILocalizationService loc, UserLadderStanding s)
    {
        if (s.Mode != LadderMode.Requirements)
            return string.Empty;

        var current = s.CurrentLevel();
        var target = s.NextLevel();
        bool collected = IsProgressCollected(s);
        int? total = target?.Requirements?.Count;
        int? met = target?.MetCount;
        int? remaining = total is int totalCount && met is int metCount ? Math.Max(0, totalCount - metCount) : null;

        bool showStatusLine = current is not null && collected && (target is null || remaining is not null);
        if (!showStatusLine)
            return string.Empty;

        return target is null
            ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BANNER_TOP))
            : remaining == 0
                ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_STATUS_AWAITING), target.Name)
                : loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_STATUS_LEFT), remaining!.Value, target.Name);
    }

    /// <summary>
    /// Profile-header status line in points mode: the points left to the next level, or the
    /// top-level text. Retention of the current level is not shown.
    /// </summary>
    internal static string HeaderPointsStatus(ILocalizationService loc, UserLadderStanding s)
    {
        if (s.Mode != LadderMode.Points || !IsProgressCollected(s) || s.Score is not decimal score || s.CurrentLevel() is null)
            return string.Empty;

        var next = s.NextLevel();
        if (next is null)
            return loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BANNER_TOP));

        if (next.Threshold is not int reach || reach <= 0)
            return string.Empty;

        // the threshold is reached but the promotion waits for the period end
        return score >= reach
            ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_STATUS_AWAITING), next.Name)
            : loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_HEADER_REACH), Points(reach - score), next.Name);
    }

    /// <summary>
    /// Top-bar progress towards the next level in percent (0–100): met/total requirements of
    /// the next level, or score/threshold of the next level; 100 on the top level. Null when
    /// the progress is not collected or cannot be computed.
    /// </summary>
    internal static decimal? TopBarProgressPercent(UserLadderStanding s)
    {
        if (!IsProgressCollected(s) || s.CurrentLevel() is null)
            return null;

        var next = s.NextLevel();
        if (next is null)
            return 100m;

        if (s.Mode == LadderMode.Requirements)
        {
            return next.Requirements?.Count is int total && total > 0 && next.MetCount is int met
                ? Math.Clamp(met * 100m / total, 0m, 100m)
                : null;
        }

        return s.Score is decimal score && next.Threshold is int threshold && threshold > 0
            ? Math.Clamp(score * 100m / threshold, 0m, 100m)
            : null;
    }

    private static string Points(decimal value) => AchievementValueFormat.Trim(decimal.Ceiling(Math.Max(0m, value)));
}
