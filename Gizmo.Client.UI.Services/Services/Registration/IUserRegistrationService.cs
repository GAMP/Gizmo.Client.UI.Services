namespace Gizmo.Client.UI.Services
{
    public interface IUserRegistrationService
    {
        Task<IReadOnlyList<RegistrationProvider>> GetProvidersAsync(CancellationToken ct = default);
        Task<RegistrationRequiredInfo?> GetRequiredUserInfoAsync(CancellationToken ct = default);
        Task<IReadOnlyList<RegistrationAgreement>> GetAgreementsAsync(CancellationToken ct = default);
        Task<bool> ExistsAsync(string value, CancellationToken ct = default);
        Task<RegistrationStartResult> StartAsync(RegistrationStartRequest request, CancellationToken ct = default);
        Task<RegistrationConfirmCode> ConfirmTokenAsync(string token, string confirmationCode, CancellationToken ct = default);
        Task<TokenConfirmedResult> IsTokenConfirmedAsync(string token, CancellationToken ct = default);
        Task<RegistrationCompleteCode> CompleteAsync(RegistrationCompleteRequest request, CancellationToken ct = default);
    }
}
