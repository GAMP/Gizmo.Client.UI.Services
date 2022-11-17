using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    public interface IClientDialogService : IDialogService
    {
        Task<ShowDialogResult<EmptyDialogResult>> ShowCheckoutDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowExecutableSelectorDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<UserAgreementResult>> ShowUserAgreementDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowTopUpDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangeEmailDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangeMobileDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangePasswordDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangePictureDialogAsync(CancellationToken cancellationToken = default);
    }
}