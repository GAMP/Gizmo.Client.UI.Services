namespace Gizmo.Client.UI.Services;

public sealed class PasswordRecoveryProvider
{
    public Guid PublicId { get; init; }
    public string Name { get; init; } = string.Empty;
    public PasswordRecoveryChannel Channel { get; init; }
    public bool IsPrimary { get; init; }
}
