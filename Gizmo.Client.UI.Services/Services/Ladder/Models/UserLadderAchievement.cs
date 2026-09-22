namespace Gizmo.Client.UI.Services;

/// <summary>
/// One achievement referenced by the standing's level requirements, with the user's live
/// completion count — listed once per standing and joined to requirement rows by id.
/// </summary>
public sealed class UserLadderAchievement
{
    /// <summary>
    /// Achievement id.
    /// </summary>
    public int AchievementId { get; init; }

    /// <summary>
    /// The achievement's display name, shown as is.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Completions earned within the current ladder period; null when progress was not
    /// collected or the user is frozen — null never means zero.
    /// </summary>
    public int? CompletedCount { get; init; }

    /// <summary>
    /// Hidden from customers: the achievements tab does not list the achievement until it is
    /// earned, so the ladder shows its name without a link.
    /// </summary>
    public bool IsHidden { get; init; }
}
