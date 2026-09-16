namespace Gizmo.Client.UI.Services;

/// <summary>
/// One challenge of the logged-in user — catalog facts, requirement standing, rewards and the
/// user's earned completions. Local projection of the user challenges API payload.
/// </summary>
public sealed class UserChallenge
{
    public int ChallengeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    /// <summary>State for this user, computed on the server.</summary>
    public ChallengeState State { get; init; }
    /// <summary>Completions the user earned on this challenge.</summary>
    public int CompletionsEarned { get; init; }
    /// <summary>Per-user completion cap; null when unlimited.</summary>
    public int? MaxCompletions { get; init; }
    /// <summary>Global prize pool size; null when uncapped.</summary>
    public int? GlobalMaxCompletions { get; init; }
    /// <summary>Prizes still available in the global pool; null when uncapped.</summary>
    public int? GlobalRemaining { get; init; }
    /// <summary>Window start (UTC); null when open-ended.</summary>
    public DateTime? StartTime { get; init; }
    /// <summary>Window end (UTC); null for an evergreen challenge.</summary>
    public DateTime? EndTime { get; init; }
    /// <summary>AND-ed requirements with the referenced achievement's name and state folded in.</summary>
    public IReadOnlyList<UserChallengeRequirement> Requirements { get; init; } = Array.Empty<UserChallengeRequirement>();
    /// <summary>Configured rewards (points, time, product) in configuration order, with the status from the latest completion when any.</summary>
    public IReadOnlyList<UserChallengeReward> Rewards { get; init; } = Array.Empty<UserChallengeReward>();
    /// <summary>Requirements fully met toward the next completion; null when progress was not collected.</summary>
    public int? MetCount { get; init; }
    /// <summary>Fractional progress (0–100) toward the next completion; null when not collected.</summary>
    public decimal? Progress { get; init; }
}
