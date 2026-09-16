namespace Gizmo.Client.UI.Services;

/// <summary>
/// One configured reward of a challenge. <see cref="Amount"/> is points for <see cref="ChallengeRewardKind.Points"/>,
/// seconds for <see cref="ChallengeRewardKind.Time"/>, quantity for <see cref="ChallengeRewardKind.Product"/>.
/// </summary>
public sealed class UserChallengeReward
{
    public ChallengeRewardKind Kind { get; init; }
    public int Amount { get; init; }
    /// <summary>Product id for product rewards; null otherwise. No product name is available on the user API.</summary>
    public int? ProductId { get; init; }
    /// <summary>Grant status of this reward in the user's latest completion; null when the user has no completion or the completion has no matching reward.</summary>
    public ChallengeRewardStatus? Status { get; init; }
}
