namespace Gizmo.Client.UI.Services;

/// <summary>
/// One ladder level within the standing, with the user's live completion of it.
/// </summary>
public sealed class UserLadderLevel
{
    /// <summary>
    /// The level's position on the ladder — higher rank is a higher level; may start at 0.
    /// </summary>
    public int Rank { get; init; }

    /// <summary>
    /// The level's display name — the user group name set by the operator, shown as is.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Optional customer-facing level description; null or empty when none.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Score required to reach the level (points mode); null in requirements mode.
    /// </summary>
    public int? Threshold { get; init; }

    /// <summary>
    /// The user's live completion of this level in percent (0–100); null when not collected or frozen.
    /// </summary>
    public decimal? Progress { get; init; }

    /// <summary>
    /// Requirements fully met this period (requirements mode); null when not collected or frozen.
    /// </summary>
    public int? MetCount { get; init; }

    /// <summary>
    /// Whether the evaluator would award this level right now; null when not collected or frozen.
    /// </summary>
    public bool? IsSatisfied { get; init; }
}
