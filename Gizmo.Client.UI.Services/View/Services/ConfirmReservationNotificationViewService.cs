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
    public sealed class ConfirmReservationNotificationViewService : ValidatingViewStateServiceBase<ConfirmReservationNotificationViewState>
    {
        public ConfirmReservationNotificationViewService(ConfirmReservationNotificationViewState viewState,
            ILogger<ConfirmReservationNotificationViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService,
            IClientNotificationService clientNotificationService,
            ConfirmReservationDialogViewService confirmReservationDialogViewService)
            : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _clientNotificationService = clientNotificationService;
            _confirmReservationDialogViewService = confirmReservationDialogViewService;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        private readonly IClientNotificationService _clientNotificationService;
        private readonly ConfirmReservationDialogViewService _confirmReservationDialogViewService;

        private AddNotificationResult<EmptyComponentResult>? _confirmReservationNotification = null;

        public void SetPin(string value)
        {
            ViewState.Pin = value;
            ViewState.RaiseChanged();
        }

        public async Task ConfirmAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                await Task.Delay(5000);
                var result = await _gizmoClient.ReservationCurrentConfirmAsync(ViewState.Pin);
                if (result == ReservationCurrentConfirmResult.Success)
                {
                    var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewService>();

                    if (hostReservationViewService.ViewState.ReservationPaymentStatus == ReservationPaymentStatus.NotRequired ||
                        hostReservationViewService.ViewState.ReservationPaymentStatus == ReservationPaymentStatus.Satisfied)
                    {
                        _clientNotificationService.TryAcknowledge(_confirmReservationNotification.Controller.Identifier);
                    }
                    else
                    {
                        ViewState.Step = 1;
                        ViewState.RaiseChanged();
                    }
                }
                else
                {
                    SetPin(string.Empty);
                    //TODO: AAAAA SHOW ERROR
                }
            }
            catch (Exception ex)
            {
            }

            ViewState.IsLoading = false;
            ViewState.RaiseChanged();
        }

        public void Ignore()
        {

        }

        public async Task OpenPaymentDialogAsync()
        {
            _clientNotificationService.TryAcknowledge(_confirmReservationNotification.Controller.Identifier);
            await _confirmReservationDialogViewService.StartAsync();
        }

        public async Task StartAsync(CancellationToken cToken = default)
        {
            if (_confirmReservationNotification != null)
                return;

            Clear();


            var result = await _gizmoClient.ReservationCurrentConfirmedAsync(cToken);

            if (result == ReservationCurrentConfirmedResult.Confirmed)
            {
                var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewState>();

                if (hostReservationViewService.ReservationPaymentStatus == Web.Api.Models.ReservationPaymentStatus.NotRequired ||
                    hostReservationViewService.ReservationPaymentStatus == Web.Api.Models.ReservationPaymentStatus.Satisfied)
                {
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
                //No reservation
                return;
            }

            try
            {
                _confirmReservationNotification = await _clientNotificationService.ShowConfirmReservationNotification();
                if (_confirmReservationNotification.Result == AddComponentResultCode.Opened)
                    await _confirmReservationNotification.WaitForResultAsync(cToken);

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
            ViewState.Pin = null;
            ViewState.IsLoading = false;

            ViewState.RaiseChanged();
        }
    }
}
