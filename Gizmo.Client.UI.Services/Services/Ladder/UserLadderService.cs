using Gizmo.Web.Api.Models;
using Gizmo.Web.Api.User.Clients;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services;

public sealed class UserLadderService : IUserLadderService
{
    // one page covers every realistic ladder career; there is no paging UI on the client
    private const int TransitionsPageSize = 100;

    private readonly AchievementLadderWebApiClient _client;
    private readonly ILogger<UserLadderService> _logger;

    public UserLadderService(AchievementLadderWebApiClient client, ILogger<UserLadderService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<UserLadderStanding?> GetStandingAsync(CancellationToken ct = default)
    {
        try
        {
            // default filter: Progress = true (live measurement)
            var result = await _client.GetStandingAsync(null, ct);

            // null = no level to display — an empty state, not an error
            return result is null ? null : UserLadderStandingMapper.Map(result);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load user ladder standing.");
            throw;
        }
    }

    // failures are not logged here: the view service logs them as a warning together with the fallback it applies
    public async Task<IReadOnlyList<UserLadderTransition>> GetTransitionsAsync(CancellationToken ct = default)
    {
        var filter = new UserAchievementLadderEventsFilter();
        filter.Pagination.Limit = TransitionsPageSize;
        filter.Pagination.SortBy = nameof(UserAchievementLadderEventModel.CreatedTime);
        filter.Pagination.IsAsc = false;

        var result = await _client.GetEventsAsync(filter, ct);

        if (result?.Data is null)
            return Array.Empty<UserLadderTransition>();

        // the server sorts by the requested column; re-order defensively so the projection can rely on it
        return result.Data
            .Select(UserLadderStandingMapper.Map)
            .OrderByDescending(transition => transition.Time)
            .ToList();
    }
}
