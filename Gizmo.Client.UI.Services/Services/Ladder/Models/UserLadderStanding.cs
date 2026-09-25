namespace Gizmo.Client.UI.Services;

/// <summary>
/// The logged-in user's position on the achievement ladder — the ladder's period, the
/// current level, the live score and every level with its completion. Local projection of
/// the user ladder standing API payload.
/// </summary>
public sealed class UserLadderStanding
{
    /// <summary>
    /// The ladder scoring mode; the client renders score and thresholds in points mode only.
    /// </summary>
    public LadderMode Mode { get; init; }

    /// <summary>
    /// The standing's state within the current period; null when progress was not collected
    /// or the user is frozen.
    /// </summary>
    public LadderStandingState? State { get; init; }

    /// <summary>
    /// The user is tier-exempt: the level never moves and nothing is measured.
    /// </summary>
    public bool IsFrozen { get; init; }

    /// <summary>
    /// The ladder's calendar period kind.
    /// </summary>
    public LadderPeriod PeriodKind { get; init; }

    /// <summary>
    /// Start of the current ladder period (UTC).
    /// </summary>
    public DateTime PeriodStart { get; init; }

    /// <summary>
    /// End of the current ladder period (UTC) — the requalify-by moment.
    /// </summary>
    public DateTime PeriodEnd { get; init; }

    /// <summary>
    /// Rank of the user's current level; expected to match one of <see cref="Levels"/>.
    /// </summary>
    public int CurrentRank { get; init; }

    /// <summary>
    /// Rank the user would land on if the period ended now; null when not collected or frozen.
    /// </summary>
    public int? ProjectedRank { get; init; }

    /// <summary>
    /// The user's live score within the current period (points mode); null in requirements
    /// mode, when not collected, or frozen — null never means zero.
    /// </summary>
    public decimal? Score { get; init; }

    /// <summary>
    /// The previous period's final score; null when unavailable.
    /// </summary>
    public decimal? LastScore { get; init; }

    /// <summary>
    /// Every level on the ladder with the user's live completion, rank ascending.
    /// </summary>
    public IReadOnlyList<UserLadderLevel> Levels { get; init; } = Array.Empty<UserLadderLevel>();

    /// <summary>
    /// Every achievement referenced by the levels' requirements, name-ordered; empty in points
    /// mode. Requirement rows join it by achievement id.
    /// </summary>
    public IReadOnlyList<UserLadderAchievement> Achievements { get; init; } = Array.Empty<UserLadderAchievement>();

    /// <summary>
    /// The newest level transitions shipped with the standing (at most three, newest first);
    /// the fallback history source when the full transition log cannot be loaded.
    /// </summary>
    public IReadOnlyList<UserLadderTransition> Transitions { get; init; } = Array.Empty<UserLadderTransition>();
}
