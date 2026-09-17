namespace Gizmo.Client.UI.Services;

public interface IUserLadderService
{
    /// <summary>
    /// Gets the logged-in user's ladder standing with live progress. Returns null when there is
    /// nothing to display (no enabled ladder, the user's group is not a level, or a guest).
    /// Throws on transport/server failure — the caller decides how to present the error.
    /// </summary>
    Task<UserLadderStanding?> GetStandingAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets the newest page of the user's level transitions, newest first (at most 100).
    /// Throws on transport/server failure — the caller decides how to present the error.
    /// </summary>
    Task<IReadOnlyList<UserLadderTransition>> GetTransitionsAsync(CancellationToken ct = default);
}
