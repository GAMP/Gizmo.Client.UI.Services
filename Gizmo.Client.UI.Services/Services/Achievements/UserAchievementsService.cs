using Gizmo.Client.Options;
using Gizmo.Web.Api.User.Clients;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services;

public sealed class UserAchievementsService : IUserAchievementsService
{
    private readonly AchievementsWebApiClient _client;
    private readonly IOptions<ClientNetworkOptions> _networkOptions;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<UserAchievementsService> _logger;
    private string? _filesBaseUrl;

    public UserAchievementsService(
        AchievementsWebApiClient client,
        IOptions<ClientNetworkOptions> networkOptions,
        NavigationManager navigationManager,
        ILogger<UserAchievementsService> logger)
    {
        _client = client;
        _networkOptions = networkOptions;
        _navigationManager = navigationManager;
        _logger = logger;
    }

    public async Task<IReadOnlyList<UserAchievement>> GetAchievementsAsync(CancellationToken ct = default)
    {
        try
        {
            // default filter: Progress = true, IncludeUnavailable = true
            var result = await _client.GetAchievementsAsync(null, ct);
            var filesBaseUrl = GetFilesBaseUrl();
            return result.Achievements.Select(a => UserAchievementMapper.Map(a, filesBaseUrl)).ToList();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load user achievements.");
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
}
