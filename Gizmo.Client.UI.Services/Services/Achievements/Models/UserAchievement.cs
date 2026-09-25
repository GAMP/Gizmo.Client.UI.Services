namespace Gizmo.Client.UI.Services;

/// <summary>
/// One achievement of the logged-in user — catalog facts plus the user's standing.
/// Local projection of the user achievements API payload.
/// </summary>
public sealed class UserAchievement
{
    /// <summary>
    /// Achievement id.
    /// </summary>
    public int AchievementId { get; init; }

    /// <summary>
    /// The achievement's display name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Customer-facing description; null or empty when the achievement has none.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Absolute url of the badge image on the server files endpoint, ready for an <c>img src</c>.
    /// Null when the achievement has no image.
    /// </summary>
    public string? ImageUrl { get; init; }

    /// <summary>
    /// True when the badge image is an svg (rendered as the whole badge), false for a raster
    /// emblem placed inside the built-in badge template or when there is no image.
    /// </summary>
    public bool ImageIsSvg { get; init; }

    /// <summary>
    /// The measurement unit of <see cref="CurrentValue"/> and <see cref="TargetValue"/> —
    /// drives their display formatting. Null when the signal has no registered provider.
    /// </summary>
    public AchievementSignalUnit? Unit { get; init; }

    /// <summary>
    /// The calendar range one completion is evaluated within.
    /// </summary>
    public AchievementPeriod Range { get; init; }

    /// <summary>
    /// The achievement's target value per completion, in the signal's native unit.
    /// </summary>
    public decimal TargetValue { get; init; }

    /// <summary>
    /// Maximum completions within one range instance.
    /// </summary>
    public int MaxCompletionsPerRange { get; init; }

    /// <summary>
    /// Hidden from customers — listed only because the user has completions on it.
    /// </summary>
    public bool IsHidden { get; init; }

    /// <summary>
    /// The achievement's state for this user, computed on the server.
    /// </summary>
    public AchievementState State { get; init; }

    /// <summary>
    /// Completions the user earned over the achievement's lifetime, including live
    /// completions of the current instance not yet recorded.
    /// </summary>
    public int TotalCompletions { get; init; }

    /// <summary>
    /// Start of the current range instance (UTC).
    /// </summary>
    public DateTime InstanceStart { get; init; }

    /// <summary>
    /// End of the current range instance (UTC, exclusive) — when earning resets.
    /// </summary>
    public DateTime InstanceEnd { get; init; }

    /// <summary>
    /// Completions earned within the current range instance, including live completions
    /// not yet recorded when progress was collected; recorded completions only otherwise.
    /// </summary>
    public int InstanceCompletions { get; init; }

    /// <summary>
    /// The user's raw signal value within the current range instance, in the signal's
    /// native unit. Null when not collected, when the instance was already at its cap
    /// before measurement, or when the signal is unmeasurable.
    /// </summary>
    public decimal? CurrentValue { get; init; }

    /// <summary>
    /// Fractional progress (0–100) toward the next completion of the current instance.
    /// Null when not collected, at the instance cap, or unmeasurable — never zero to mean
    /// "not collected".
    /// </summary>
    public decimal? Progress { get; init; }
}
