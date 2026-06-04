namespace Gizmo.Client.UI.Services;

public interface IPasswordRecoveryService
{
    Task<IReadOnlyList<PasswordRecoveryProvider>> GetProvidersAsync(CancellationToken ct = default);

    Task<PasswordRecoveryStartResult> StartAsync(PasswordRecoveryStartRequest request, CancellationToken ct = default);

    Task<PasswordRecoveryConfirmCode> ConfirmCodeAsync(string token, string confirmationCode, CancellationToken ct = default);

    Task<PasswordRecoveryCompleteCode> CompleteAsync(string token, string newPassword, CancellationToken ct = default);
}
