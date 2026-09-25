namespace Gizmo.Client.UI.Services;

/// <summary>
/// One challenge of the logged-in user — catalog facts, requirement standing, rewards and the
/// user's earned completions. Local projection of the user challenges API payload.
/// </summary>
public sealed class UserChallenge
{
    /// <summary>
    /// Challenge id.
    /// </summary>
    public int ChallengeId { get; init; }

    /// <summary>
    /// The challenge's display name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Customer-facing description; null or empty when the challenge has none.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The challenge's state for this user, computed on the server.
    /// </summary>
    public ChallengeState State { get; init; }

    /// <summary>
    /// Completions the user earned on this challenge.
    /// </summary>
    public int CompletionsEarned { get; init; }

    /// <summary>
    /// Maximum completions one user may earn; null when unlimited.
    /// </summary>
    public int? MaxCompletions { get; init; }

    /// <summary>
    /// Size of the global prize pool shared by all users; null when uncapped.
    /// </summary>
    public int? GlobalMaxCompletions { get; init; }

    /// <summary>
    /// Prizes still available in the global pool; null when uncapped.
    /// </summary>
    public int? GlobalRemaining { get; init; }

    /// <summary>
    /// Start of the challenge window (UTC); null when the challenge has no start.
    /// </summary>
    public DateTime? StartTime { get; init; }

    /// <summary>
    /// End of the challenge window (UTC); null for an evergreen challenge.
    /// </summary>
    public DateTime? EndTime { get; init; }

    /// <summary>
    /// AND-ed requirements toward one completion, with the referenced achievement's
    /// name and state folded in.
    /// </summary>
    public IReadOnlyList<UserChallengeRequirement> Requirements { get; init; } = Array.Empty<UserChallengeRequirement>();

    /// <summary>
    /// Configured rewards (points, time, product) in configuration order, each carrying
    /// the grant status from the user's latest completion when there is one.
    /// </summary>
    public IReadOnlyList<UserChallengeReward> Rewards { get; init; } = Array.Empty<UserChallengeReward>();

    /// <summary>
    /// Requirements fully met toward the next completion; null when progress was not collected.
    /// </summary>
    public int? MetCount { get; init; }

    /// <summary>
    /// Progress toward the next completion in percent (0–100), the weakest requirement gates it;
    /// null when progress was not collected.
    /// </summary>
    public decimal? Progress { get; init; }
}
