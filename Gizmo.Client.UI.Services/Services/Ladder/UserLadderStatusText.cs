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

    internal static string HeaderPointsStatus(ILocalizationService loc, UserLadderStanding s)
    {
        if (s.Mode != LadderMode.Points || !IsProgressCollected(s) || s.Score is not decimal score)
            return string.Empty;

        var current = s.CurrentLevel();
        if (current is null)
            return string.Empty;

        var next = s.NextLevel();
        int? reach = next?.Threshold is int r && r > 0 ? r : null;

        // a single-level ladder is at entry and at the top at once: it falls through to the secured rule
        if (s.IsAtEntry() && next is not null)
        {
            return reach is int reachValue
                ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_HEADER_REACH), Points(reachValue - score), next.Name)
                : string.Empty;
        }

        if (s.IsSecured())
        {
            if (next is null)
                return loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_HEADER_SECURED_TOP));

            return reach is int reachValue2
                ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_HEADER_SECURED_NEXT), Points(reachValue2 - score), next.Name)
                : string.Empty;
        }

        int? keep = !s.IsAtEntry() && current.Threshold is int k && k > 0 ? k : null;
        return keep is int keepValue
            ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_HEADER_RETAIN), Points(keepValue - score), current.Name)
            : string.Empty;
    }

    internal static string TopBarProgress(ILocalizationService loc, UserLadderStanding s)
    {
        if (!IsProgressCollected(s) || s.CurrentLevel() is null)
            return string.Empty;

        var next = s.NextLevel();
        if (next is null)
            return loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BANNER_TOP));

        if (s.Mode == LadderMode.Requirements)
        {
            int? total = next.Requirements?.Count;
            int? met = next.MetCount;
            return total is int totalCount && totalCount > 0 && met is int metCount
                ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_TOPBAR_NEXT_MET), next.Name, metCount, totalCount)
                : string.Empty;
        }

        return s.Score is decimal score && next.Threshold is int threshold && threshold > 0
            ? loc.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_TOPBAR_NEXT_POINTS), next.Name, AchievementValueFormat.Trim(decimal.Floor(score)), AchievementValueFormat.Trim(threshold))
            : string.Empty;
    }

    private static string Points(decimal value) => AchievementValueFormat.Trim(decimal.Ceiling(Math.Max(0m, value)));
}
