namespace Gizmo.Client.UI.Services;

/// <summary>
/// One level change of the user — from the standing's newest transitions or the ladder event log.
/// </summary>
public sealed class UserLadderTransition
{
    /// <summary>
    /// When the transition happened (UTC).
    /// </summary>
    public DateTime Time { get; init; }

    /// <summary>
    /// The rank moved from, as it was at the time; may no longer exist on the ladder.
    /// </summary>
    public int FromRank { get; init; }

    /// <summary>
    /// The rank moved to, as it was at the time; may no longer exist on the ladder.
    /// </summary>
    public int ToRank { get; init; }

    /// <summary>
    /// What caused the transition.
    /// </summary>
    public LadderTransitionTrigger Trigger { get; init; }
}
