namespace Gizmo.Client.UI.Services;

public interface IAuthenticationService
{
    Task<AuthenticationLoginResult> LoginAsync(AuthenticationLoginRequest request, CancellationToken cancellationToken = default);
}
