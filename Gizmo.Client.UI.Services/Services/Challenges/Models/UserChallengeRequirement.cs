namespace Gizmo.Client.UI.Services;

/// <summary>
/// One requirement of a challenge: an achievement that must be completed a number of times
/// within the challenge window.
/// </summary>
public sealed class UserChallengeRequirement
{
    /// <summary>
    /// Id of the required achievement.
    /// </summary>
    public int AchievementId { get; init; }

    /// <summary>
    /// The achievement's display name; empty when the achievement is not part of the
    /// challenges view (e.g. deleted).
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The achievement's state for this user, computed on the server; <see cref="AchievementState.Active"/>
    /// when the achievement is not part of the challenges view.
    /// </summary>
    public AchievementState AchievementState { get; init; }

    /// <summary>
    /// Achievement completions needed to satisfy this requirement.
    /// </summary>
    public int RequiredCount { get; init; }

    /// <summary>
    /// Achievement completions counted toward this requirement; null when progress was not collected.
    /// </summary>
    public int? CompletedCount { get; init; }

    /// <summary>
    /// Progress toward this requirement in percent (0–100); null when progress was not collected.
    /// </summary>
    public decimal? Progress { get; init; }

    /// <summary>
    /// Achievement completions still earnable before the challenge window closes;
    /// null when progress was not collected or the window has no end.
    /// </summary>
    public int? RemainingAchievable { get; init; }

    /// <summary>
    /// True when <see cref="CompletedCount"/> reached <see cref="RequiredCount"/>;
    /// false when progress was not collected.
    /// </summary>
    public bool IsMet => CompletedCount >= RequiredCount;
}
