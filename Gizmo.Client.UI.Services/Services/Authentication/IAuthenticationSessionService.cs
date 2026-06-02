namespace Gizmo.Client.UI.Services;

public interface IAuthenticationSessionService
{
    event EventHandler? Changed;

    AuthenticationState State { get; }
    AuthenticationLoginError LastError { get; }
    string? LastErrorMessage { get; }
    string? Username { get; }

    void SetLoggingIn();
    void SetLoggedIn(string? username);
    void SetLoginFailed(AuthenticationLoginError error, string? message);
    void Clear();
}
