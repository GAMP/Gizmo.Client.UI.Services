namespace Gizmo.Client.UI.Services;

public sealed class AuthenticationLoginRequest
{
    public string Username { get; init; } = string.Empty;
    public string? Password { get; init; }
    public string? Pin { get; init; }
}
