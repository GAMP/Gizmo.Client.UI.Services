using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    public interface IClientDialogService : IDialogService
    {
        Task<ShowDialogResult<EmptyDialogResult>> ShowCheckoutDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowExecutableSelectorDialogAsync(int applicationId, CancellationToken cancellationToken = default);
        Task<ShowDialogResult<UserAgreementResult>> ShowUserAgreementDialogAsync(UserAgreementDialogParameters userAgreementDialogParameters, CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangeProfileDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangeEmailDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangeMobileDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangePasswordDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowChangePictureDialogAsync(CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowPaymentDialogAsync(PaymentDialogParameters paymentDialogParameters, CancellationToken cancellationToken = default);
        Task<ShowDialogResult<EmptyDialogResult>> ShowAdvertisementDialogAsync(AdvertisementViewState state, CancellationToken cancellationToken = default);
    }
}
