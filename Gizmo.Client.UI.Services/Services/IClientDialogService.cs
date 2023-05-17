using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    public interface IClientDialogService : IDialogService
    {
        Task<ShowDialogResult<EmptyDialogResult>> ShowCheckoutDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<UserAgreementResult>> ShowUserAgreementDialogAsync(UserAgreementDialogParameters userAgreementDialogParameters, CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangeProfileDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangeEmailDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangeMobileDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangePasswordDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangePictureDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowMediaDialogAsync(MediaDialogParameters mediaDialogParameters, CancellationToken cancellationToken = default);
        Task<ShowDialogResult<AlertDialogResult>> ShowAlertDialogAsync(string title, string message, AlertDialogButtons buttons = AlertDialogButtons.OK, AlertDialogIcons icon = AlertDialogIcons.None, CancellationToken cancellationToken = default);
    }
}
