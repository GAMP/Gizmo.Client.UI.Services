namespace Gizmo.Client.UI.Services;

public sealed class RegistrationStartRequest
{
    public int MethodId { get; init; }
    public RegistrationStartKind Kind { get; init; }
    public string? Value { get; init; }
}
