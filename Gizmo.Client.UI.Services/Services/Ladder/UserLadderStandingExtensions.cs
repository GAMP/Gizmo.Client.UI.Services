namespace Gizmo.Client.UI.Services;

/// <summary>
/// Position arithmetic over a ladder standing — lookups only, nothing the server evaluator decides.
/// </summary>
public static class UserLadderStandingExtensions
{
    /// <summary>
    /// The level whose rank equals the current rank; null when the standing's levels do not contain it.
    /// </summary>
    public static UserLadderLevel? CurrentLevel(this UserLadderStanding standing) =>
        standing.Levels.FirstOrDefault(level => level.Rank == standing.CurrentRank);

    /// <summary>
    /// The level after the current one in list order; null at the top or when the current level is missing.
    /// </summary>
    public static UserLadderLevel? NextLevel(this UserLadderStanding standing)
    {
        for (int index = 0; index < standing.Levels.Count - 1; index++)
        {
            if (standing.Levels[index].Rank == standing.CurrentRank)
                return standing.Levels[index + 1];
        }

        return null;
    }

    /// <summary>
    /// True when the ladder has a single level.
    /// </summary>
    public static bool IsOnlyLevel(this UserLadderStanding standing) => standing.Levels.Count == 1;

    /// <summary>
    /// True when the first level in list order is the current one.
    /// </summary>
    public static bool IsAtEntry(this UserLadderStanding standing) =>
        standing.Levels.Count > 0 && standing.Levels[0].Rank == standing.CurrentRank;

    /// <summary>
    /// True when the standing's state is Secured or Awaiting; false for Earning, Unknown and null.
    /// </summary>
    public static bool IsSecured(this UserLadderStanding standing) =>
        standing.State is LadderStandingState.Secured or LadderStandingState.Awaiting;

    /// <summary>
    /// The level name for a rank, or "#{rank}" when the rank is no longer on the ladder.
    /// </summary>
    public static string LevelNameByRank(this UserLadderStanding standing, int rank) =>
        standing.Levels.FirstOrDefault(level => level.Rank == rank)?.Name ?? $"#{rank}";

    /// <summary>
    /// 1-based position of the rank in the level list (ranks may start at 0); the rank itself when not found.
    /// </summary>
    public static int OrdinalOf(this UserLadderStanding standing, int rank)
    {
        for (int index = 0; index < standing.Levels.Count; index++)
        {
            if (standing.Levels[index].Rank == rank)
                return index + 1;
        }

        return rank;
    }
}
