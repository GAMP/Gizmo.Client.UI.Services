namespace Gizmo.Client.UI.Services;

public sealed class AuthenticationLoginResult
{
    public bool Success { get; init; }
    public AuthenticationLoginError Error { get; init; }
    public string? Message { get; init; }
}
