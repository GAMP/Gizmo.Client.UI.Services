namespace Gizmo.Client.UI.Services;

public sealed class PasswordRecoveryStartRequest
{
    public int MethodId { get; init; }
    public PasswordRecoveryIdentifierKind IdentifierKind { get; init; }
    public string Value { get; init; } = string.Empty;
}
