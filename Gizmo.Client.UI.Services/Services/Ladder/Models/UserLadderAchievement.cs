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
    /// The user's raw signal value within the achievement's current range instance — progress
    /// of the completion in work; null when not collected, frozen, or the signal is orphaned.
    /// </summary>
    public decimal? CurrentValue { get; init; }

    /// <summary>
    /// The achievement's target value per completion, in the unit of <see cref="Unit"/>.
    /// </summary>
    public decimal TargetValue { get; init; }

    /// <summary>
    /// The measurement unit of <see cref="CurrentValue"/> and <see cref="TargetValue"/>; null when
    /// the signal has no registered provider.
    /// </summary>
    public AchievementSignalUnit? Unit { get; init; }

    /// <summary>
    /// Hidden from customers: the achievements tab does not list the achievement until it is
    /// earned, so the ladder shows its name without a link.
    /// </summary>
    public bool IsHidden { get; init; }
}
