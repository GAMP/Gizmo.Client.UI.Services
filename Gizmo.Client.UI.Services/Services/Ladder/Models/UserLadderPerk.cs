namespace Gizmo.Client.UI.Services;

/// <summary>
/// One perk a ladder level confers — a discount of the level's discount group or the
/// level's waiting-line priority.
/// </summary>
public sealed class UserLadderPerk
{
    /// <summary>
    /// What the perk is.
    /// </summary>
    public LadderPerkKind Kind { get; init; }

    /// <summary>
    /// The discount's operator-authored display name; empty for a waiting-line perk.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The discount magnitude — percent when <see cref="IsPercentage"/>, otherwise a currency amount.
    /// </summary>
    public decimal Value { get; init; }

    /// <summary>
    /// The discount magnitude is a percentage rather than a fixed amount.
    /// </summary>
    public bool IsPercentage { get; init; }

    /// <summary>
    /// The discount is granted as a bonus instead of being taken off the price.
    /// </summary>
    public bool IsBonus { get; init; }

    /// <summary>
    /// The waiting-line priority value; 0 for a discount perk.
    /// </summary>
    public int Priority { get; init; }
}
