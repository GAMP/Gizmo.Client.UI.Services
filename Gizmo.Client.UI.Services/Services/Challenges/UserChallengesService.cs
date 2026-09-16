using Gizmo.Web.Api.Models;
using Gizmo.Web.Api.User.Clients;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services;

public sealed class UserChallengesService : IUserChallengesService
{
    private readonly AchievementsWebApiClient _client;
    private readonly ILogger<UserChallengesService> _logger;

    public UserChallengesService(AchievementsWebApiClient client, ILogger<UserChallengesService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IReadOnlyList<UserChallenge>> GetChallengesAsync(CancellationToken ct = default)
    {
        try
        {
            // default filter: Progress = true, IncludeUnavailable = true
            var result = await _client.GetChallengesAsync(null, ct);

            // null for an unknown user — nothing to show, not an error
            if (result is null)
                return Array.Empty<UserChallenge>();

            var achievements = result.Achievements.ToDictionary(achievement => achievement.AchievementId);

            return result.Challenges.Select(challenge => UserChallengeMapper.Map(challenge, achievements)).ToList();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load user challenges.");
            throw;
        }
    }
}
