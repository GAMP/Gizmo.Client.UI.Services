namespace Gizmo.Client.UI.Services;

/// <summary>
/// One configured reward of a challenge, granted on every completion.
/// </summary>
public sealed class UserChallengeReward
{
    /// <summary>
    /// What is granted — points, time or a product.
    /// </summary>
    public ChallengeRewardKind Kind { get; init; }

    /// <summary>
    /// Reward size in the kind's native unit: points for <see cref="ChallengeRewardKind.Points"/>,
    /// seconds for <see cref="ChallengeRewardKind.Time"/>, quantity for <see cref="ChallengeRewardKind.Product"/>.
    /// </summary>
    public int Amount { get; init; }

    /// <summary>
    /// Id of the granted product for <see cref="ChallengeRewardKind.Product"/>; null otherwise.
    /// The user API carries no product name.
    /// </summary>
    public int? ProductId { get; init; }

    /// <summary>
    /// Grant status of this reward in the user's latest completion; null when the user has no
    /// completion yet or the completion carries no matching reward.
    /// </summary>
    public ChallengeRewardStatus? Status { get; init; }
}
