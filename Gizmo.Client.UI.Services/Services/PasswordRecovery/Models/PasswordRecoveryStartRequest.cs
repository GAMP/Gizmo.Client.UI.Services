namespace Gizmo.Client.UI.Services;

public sealed class PasswordRecoveryStartRequest
{
    public Guid IntegrationPublicId { get; init; }
    public PasswordRecoveryChannel Channel { get; init; }
    public string MatchValue { get; init; } = string.Empty;
}
