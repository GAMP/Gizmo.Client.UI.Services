namespace Gizmo.Client.UI.Services;

public sealed class PasswordRecoveryProvider
{
    public Guid PublicId { get; init; }
    public PasswordRecoveryChannel Channel { get; init; }
}
