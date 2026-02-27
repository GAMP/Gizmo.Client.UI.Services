using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ConfirmReservationNotificationViewService : ViewStateServiceBase<ConfirmReservationNotificationViewState>
    {
        public ConfirmReservationNotificationViewService(ConfirmReservationNotificationViewState viewState,
            ILogger<ConfirmReservationNotificationViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            IClientNotificationService clientNotificationService)
            : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _clientNotificationService = clientNotificationService;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly IClientNotificationService _clientNotificationService;

        private AddNotificationResult<EmptyComponentResult>? _confirmReservationNotification = null;
        
        public void Ignore()
        {
            var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewService>();
            hostReservationViewService.Ignore();
            _confirmReservationNotification?.Controller?.Result(new EmptyComponentResult());
        }

        public void Dismiss()
        {
            var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewService>();
            hostReservationViewService.Dismiss();
            _confirmReservationNotification?.Controller?.Result(new EmptyComponentResult());
        }

        public async Task OpenPaymentDialogAsync()
        {
            var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewService>();
            _confirmReservationNotification?.Controller?.Result(new EmptyComponentResult());
            await hostReservationViewService.ShowDialog();
        }

        public void CloseIfWaitingPayment()
        {
            //Close notification if waiting payment.
            if (_confirmReservationNotification != null && ViewState.Step == 1)
            {
                _confirmReservationNotification?.Controller?.Result(new EmptyComponentResult());
            }
        }

        public async Task StartAsync(CancellationToken cToken = default)
        {
            if (_confirmReservationNotification != null)
                return;

            Clear();

            try
            {
                var result = await _gizmoClient.ReservationCurrentConfirmedAsync(cToken);

                if (result == ReservationCurrentConfirmedResult.Confirmed)
                {
                    var hostReservationViewState = ServiceProvider.GetRequiredService<HostReservationViewState>();

                    if (hostReservationViewState.ReservationPaymentStatus == Web.Api.Models.ReservationPaymentStatus.NotRequired ||
                        hostReservationViewState.ReservationPaymentStatus == Web.Api.Models.ReservationPaymentStatus.Satisfied)
                    {
                        //If confirmed and paid do no show the notification.
                        return;
                    }
                    else
                    {
                        ViewState.Step = 1;
                    }
                }
                else if (result == ReservationCurrentConfirmedResult.Unconfirmed)
                {

                }
                else
                {
                    //No reservation.
                    return;
                }

                _confirmReservationNotification = await _clientNotificationService.ShowConfirmReservationNotification(cToken);
                if (_confirmReservationNotification.Result == AddComponentResultCode.Opened)
                    await _confirmReservationNotification.WaitForResultAsync(cToken);

                _confirmReservationNotification = null;
            }
            catch (OperationCanceledException)
            {
                _confirmReservationNotification = null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to show confirm reservation notification.");
            }
        }

        public void Clear()
        {
            ViewState.Step = 0;
            ViewState.IsLoading = false;

            ViewState.RaiseChanged();
        }
    }
}
