namespace Gizmo.Client.UI.Services;

/// <summary>
/// One requirement of a challenge: an achievement that must be completed a number of times.
/// </summary>
public sealed class UserChallengeRequirement
{
    public int AchievementId { get; init; }
    /// <summary>Achievement display name; empty when the achievement is not part of the challenges view.</summary>
    public string Name { get; init; } = string.Empty;
    /// <summary>The referenced achievement's state for this user.</summary>
    public AchievementState AchievementState { get; init; }
    public int RequiredCount { get; init; }
    /// <summary>Completions counted toward this requirement; null when not collected.</summary>
    public int? CompletedCount { get; init; }
    public decimal? Progress { get; init; }
    public int? RemainingAchievable { get; init; }
    public bool IsMet => CompletedCount >= RequiredCount;
}
