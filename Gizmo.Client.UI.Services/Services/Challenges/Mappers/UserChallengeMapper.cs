using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services
{
    internal static class UserChallengeMapper
    {
        /// <param name="achievements">The view's achievements by id — requirement rows point at them by id.</param>
        internal static UserChallenge Map(UserAchievementChallengeModel source, IReadOnlyDictionary<int, UserAchievementChallengeAchievementModel> achievements)
        {
            // reward statuses live per completion; the popup shows the latest one
            var latest = source.MyCompletions.Count == 0
                ? null
                : source.MyCompletions.OrderByDescending(completion => completion.Occurrence).First();

            var rewards = new List<UserChallengeReward>();

            if (source.PointsRewards is { } points)
                rewards.AddRange(points.Select((reward, index) => new UserChallengeReward
                {
                    Kind = ChallengeRewardKind.Points,
                    Amount = reward.Amount,
                    Status = StatusAt(latest?.PointsRewards?.Select(r => r.Status), index),
                }));

            if (source.TimeRewards is { } time)
                rewards.AddRange(time.Select((reward, index) => new UserChallengeReward
                {
                    Kind = ChallengeRewardKind.Time,
                    Amount = reward.Seconds,
                    Status = StatusAt(latest?.TimeRewards?.Select(r => r.Status), index),
                }));

            if (source.ProductRewards is { } products)
                rewards.AddRange(products.Select((reward, index) => new UserChallengeReward
                {
                    Kind = ChallengeRewardKind.Product,
                    Amount = reward.Quantity,
                    ProductId = reward.ProductId,
                    Status = StatusAt(latest?.ProductRewards?.Select(r => r.Status), index),
                }));

            return new UserChallenge
            {
                ChallengeId          = source.ChallengeId,
                Name                 = source.Name,
                Description          = source.Description,
                State                = ChallengeEnumMapper.Map(source.State),
                CompletionsEarned    = source.CompletionsEarned,
                MaxCompletions       = source.MaxCompletions,
                GlobalMaxCompletions = source.GlobalMaxCompletions,
                GlobalRemaining      = source.GlobalRemaining,
                StartTime            = source.StartTime,
                EndTime              = source.EndTime,
                Requirements         = source.Requirements.Select(requirement =>
                {
                    achievements.TryGetValue(requirement.AchievementId, out var achievement);

                    return new UserChallengeRequirement
                    {
                        AchievementId       = requirement.AchievementId,
                        Name                = achievement?.Name ?? string.Empty,
                        AchievementState    = AchievementEnumMapper.Map(achievement?.State ?? UserAchievementState.Active),
                        RequiredCount       = requirement.RequiredCount,
                        CompletedCount      = requirement.CompletedCount,
                        Progress            = requirement.Progress,
                        RemainingAchievable = requirement.RemainingAchievable,
                    };
                }).ToList(),
                Rewards              = rewards,
                MetCount             = source.MetCount,
                Progress             = source.Progress,
            };
        }

        // rewards carry no id — the i-th completion reward of a kind corresponds to the i-th configured one
        private static ChallengeRewardStatus? StatusAt(IEnumerable<AchievementChallengeRewardStatus>? statuses, int index)
        {
            var status = statuses?.Skip(index).Take(1).ToList();
            return status is { Count: 1 } ? ChallengeEnumMapper.Map(status[0]) : null;
        }
    }
}
