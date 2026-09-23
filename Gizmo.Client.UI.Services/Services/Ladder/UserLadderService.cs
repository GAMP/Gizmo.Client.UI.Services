using Gizmo.Client.Options;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Api.User.Clients;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services;

public sealed class UserLadderService : IUserLadderService
{
    // one page covers every realistic ladder career; there is no paging UI on the client
    private const int TransitionsPageSize = 100;

    private readonly AchievementLadderWebApiClient _client;
    private readonly IOptions<ClientNetworkOptions> _networkOptions;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<UserLadderService> _logger;
    private string? _filesBaseUrl;

    public UserLadderService(
        AchievementLadderWebApiClient client,
        IOptions<ClientNetworkOptions> networkOptions,
        NavigationManager navigationManager,
        ILogger<UserLadderService> logger)
    {
        _client = client;
        _networkOptions = networkOptions;
        _navigationManager = navigationManager;
        _logger = logger;
    }

    public async Task<UserLadderStanding?> GetStandingAsync(CancellationToken ct = default)
    {
        try
        {
            // default filter: Progress = true (live measurement)
            var result = await _client.GetStandingAsync(null, ct);

            // null = no level to display — an empty state, not an error
            return result is null ? null : UserLadderStandingMapper.Map(result, GetFilesBaseUrl());
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

    private string GetFilesBaseUrl()
    {
        if (_filesBaseUrl is null)
        {
            var host = _networkOptions.Value.ServerUri;
            if (string.IsNullOrWhiteSpace(host))
                host = _navigationManager.BaseUri;

            _filesBaseUrl = new UriBuilder(host) { Path = "files/" }.Uri.ToString();
        }

        return _filesBaseUrl;
    }

    // failures are not logged here: the view service logs them as a warning together with the fallback it applies
    public async Task<IReadOnlyList<UserLadderTransition>> GetTransitionsAsync(CancellationToken ct = default)
    {
        // sorted by Id, not CreatedTime: the server only sorts by [Sortable] model properties and
        // UserAchievementLadderEventModel has none — Id is the identity column, so it is creation-ordered
        var filter = new UserAchievementLadderEventsFilter();
        filter.Pagination.Limit = TransitionsPageSize;
        filter.Pagination.SortBy = nameof(UserAchievementLadderEventModel.Id);
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
