namespace Gizmo.Client.UI.Services;

/// <summary>
/// One requirement of a ladder level: an achievement that must be completed a number of
/// times within the ladder period. The achievement's name and live completion live once in
/// the standing's achievements lookup, referenced by <see cref="AchievementId"/>.
/// </summary>
public sealed class UserLadderRequirement
{
    /// <summary>
    /// Id of the required achievement; resolvable in the standing's achievements lookup.
    /// </summary>
    public int AchievementId { get; init; }

    /// <summary>
    /// Achievement completions needed within the ladder period to satisfy this requirement.
    /// </summary>
    public int RequiredCount { get; init; }
}
