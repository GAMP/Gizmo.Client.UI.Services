namespace Gizmo.Client.UI.Services;

/// <summary>
/// Shared singleton holder of the logged-in user's ladder standing, so the ladder tab, the
/// profile header and the top app bar consume one load instead of one each.
/// </summary>
public interface IUserLadderStandingContext
{
    /// <summary>
    /// Raised after a successful <see cref="RefreshAsync"/> and after <see cref="Clear"/>.
    /// </summary>
    event EventHandler? Changed;

    /// <summary>
    /// The last loaded standing; null before the first load, after <see cref="Clear"/>, or when
    /// the server has no level for the user.
    /// </summary>
    UserLadderStanding? Standing { get; }

    /// <summary>
    /// Re-requests the standing and stores it, then raises <see cref="Changed"/>. On failure the
    /// previous value is kept, <see cref="Changed"/> is not raised, and the exception is rethrown.
    /// </summary>
    Task RefreshAsync(CancellationToken ct = default);

    /// <summary>
    /// Clears the stored standing and raises <see cref="Changed"/>.
    /// </summary>
    void Clear();
}
