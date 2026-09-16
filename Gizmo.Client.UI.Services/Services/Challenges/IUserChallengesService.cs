namespace Gizmo.Client.UI.Services;

public interface IUserChallengesService
{
    /// <summary>
    /// Gets the logged-in user's challenges with live requirement progress and earned completions.
    /// Throws on transport/server failure — the caller decides how to present the error.
    /// </summary>
    Task<IReadOnlyList<UserChallenge>> GetChallengesAsync(CancellationToken ct = default);
}
