using Gizmo.UI;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    public interface IClientDialogService : IDialogService
    {
        Task<AddDialogResult<EmptyComponentResult>> ShowCheckoutDialogAsync(CancellationToken cancellationToken = default);
        Task<AddDialogResult<UserAgreementResult>> ShowUserAgreementDialogAsync(UserAgreementDialogParameters userAgreementDialogParameters, CancellationToken cancellationToken = default);
        Task<AddDialogResult<EmptyComponentResult>> ShowChangeProfileDialogAsync(CancellationToken cancellationToken = default);
        Task<AddDialogResult<EmptyComponentResult>> ShowChangePasswordDialogAsync(bool closable, CancellationToken cancellationToken = default);
        Task<AddDialogResult<EmptyComponentResult>> ShowChangePictureDialogAsync(CancellationToken cancellationToken = default);
        Task<AddDialogResult<EmptyComponentResult>> ShowMediaDialogAsync(MediaDialogParameters mediaDialogParameters, CancellationToken cancellationToken = default);
        Task<AddDialogResult<AlertDialogResult>> ShowAlertDialogAsync(string title, string message, AlertDialogButtons buttons = AlertDialogButtons.OK, AlertTypes icon = AlertTypes.None, CancellationToken cancellationToken = default);
        Task<AddDialogResult<EmptyComponentResult>> ShowUserOnlineDepositsDialogAsync(CancellationToken cancellationToken = default);
        Task<AddDialogResult<EmptyComponentResult>> ShowConfirmReservationDialogAsync(CancellationToken cancellationToken = default);
        Task<AddDialogResult<RegistrationAgreementsResult>> ShowRegistrationAgreementsDialogAsync(IReadOnlyList<RegistrationAgreement> agreements, CancellationToken cancellationToken = default);
    }
}
