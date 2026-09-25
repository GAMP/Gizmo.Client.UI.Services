using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

internal static class ChallengeEnumMapper
{
    internal static ChallengeState Map(UserAchievementChallengeState source) => source switch
    {
        UserAchievementChallengeState.Active        => ChallengeState.Active,
        UserAchievementChallengeState.NotStarted    => ChallengeState.NotStarted,
        UserAchievementChallengeState.Unmeasurable  => ChallengeState.Unmeasurable,
        UserAchievementChallengeState.Blocked       => ChallengeState.Blocked,
        UserAchievementChallengeState.Paused        => ChallengeState.Paused,
        UserAchievementChallengeState.Unreachable   => ChallengeState.Unreachable,
        UserAchievementChallengeState.Ineligible    => ChallengeState.Ineligible,
        UserAchievementChallengeState.PoolExhausted => ChallengeState.PoolExhausted,
        UserAchievementChallengeState.Ended         => ChallengeState.Ended,
        UserAchievementChallengeState.Archived      => ChallengeState.Archived,
        UserAchievementChallengeState.Done          => ChallengeState.Done,
        _                                           => ChallengeState.Unknown,
    };

    internal static ChallengeRewardStatus Map(AchievementChallengeRewardStatus source) => source switch
    {
        AchievementChallengeRewardStatus.Pending       => ChallengeRewardStatus.Pending,
        AchievementChallengeRewardStatus.AwaitingClaim => ChallengeRewardStatus.AwaitingClaim,
        AchievementChallengeRewardStatus.Delivered     => ChallengeRewardStatus.Delivered,
        AchievementChallengeRewardStatus.Declined      => ChallengeRewardStatus.Declined,
        AchievementChallengeRewardStatus.Claimed       => ChallengeRewardStatus.Claimed,
        _                                              => ChallengeRewardStatus.Unknown,
    };
}
