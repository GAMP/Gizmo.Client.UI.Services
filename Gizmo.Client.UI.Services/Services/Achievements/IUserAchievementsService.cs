namespace Gizmo.Client.UI.Services;

public interface IUserAchievementsService
{
    /// <summary>
    /// Gets the logged-in user's achievements with live progress. Throws on transport/server failure —
    /// the caller decides how to present the error.
    /// </summary>
    Task<IReadOnlyList<UserAchievement>> GetAchievementsAsync(CancellationToken ct = default);
}
