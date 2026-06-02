namespace Gizmo.Client.UI.Services;

public sealed class AuthenticationSessionService : IAuthenticationSessionService
{
    public event EventHandler? Changed;

    public AuthenticationState State { get; private set; }
    public AuthenticationLoginError LastError { get; private set; }
    public string? LastErrorMessage { get; private set; }
    public string? Username { get; private set; }

    public void SetLoggingIn()
    {
        State = AuthenticationState.LoggingIn;
        LastError = AuthenticationLoginError.None;
        LastErrorMessage = null;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void SetLoggedIn(string? username)
    {
        State = AuthenticationState.LoggedIn;
        LastError = AuthenticationLoginError.None;
        LastErrorMessage = null;
        Username = username;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void SetLoginFailed(AuthenticationLoginError error, string? message)
    {
        State = AuthenticationState.LoginFailed;
        LastError = error;
        LastErrorMessage = message;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Clear()
    {
        State = AuthenticationState.LoggedOut;
        LastError = AuthenticationLoginError.None;
        LastErrorMessage = null;
        Username = null;
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
