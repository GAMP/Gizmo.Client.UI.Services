namespace Gizmo.Client.UI.Services;

public interface IPasswordRecoveryService
{
    Task<IReadOnlyList<PasswordRecoveryProvider>> GetConfiguredMethodsAsync(CancellationToken ct = default);

    Task<IReadOnlyList<PasswordRecoveryProvider>> GetMethodsAsync(PasswordRecoveryIdentifierKind identifierKind, string value, CancellationToken ct = default);

    Task<PasswordRecoveryStartResult> StartAsync(PasswordRecoveryStartRequest request, CancellationToken ct = default);

    Task<PasswordRecoveryConfirmCode> ConfirmCodeAsync(string token, string confirmationCode, CancellationToken ct = default);

    Task<TokenConfirmedResult> IsTokenConfirmedAsync(string token, CancellationToken ct = default);

    Task<PasswordRecoveryCompleteCode> CompleteAsync(string token, string newPassword, CancellationToken ct = default);
}
