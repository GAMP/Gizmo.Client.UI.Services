using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components.Forms;
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
            ViewState.ErrorMessage = null;
            ViewState.Pin = value;
            ValidateProperty(() => ViewState.Pin);
        }

        public async Task ConfirmAsync()
        {
            ViewState.ErrorMessage = null;

            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                var result = await _gizmoClient.ReservationCurrentConfirmAsync(ViewState.Pin);
                if (result == ReservationCurrentConfirmResult.Success)
                {
                    var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewService>();

                    if (hostReservationViewService.ViewState.ReservationPaymentStatus == ReservationPaymentStatus.NotRequired ||
                        hostReservationViewService.ViewState.ReservationPaymentStatus == ReservationPaymentStatus.Satisfied)
                    {
                        //If confirmed and paid close the notification.
                        _confirmReservationNotification?.Controller?.Result(new EmptyComponentResult());
                    }
                    else
                    {
                        ViewState.Step = 1;
                    }
                }
                else
                {
                    ViewState.Pin = null;
                    ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURED));
                }
            }
            catch (Exception ex)
            {
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURED));
            }

            ViewState.IsLoading = false;
            ViewState.RaiseChanged();
        }

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
            hostReservationViewService.Ignore();
            _confirmReservationNotification?.Controller?.Result(new EmptyComponentResult());
            await hostReservationViewService.ShowDialog();
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
            ViewState.Pin = null;
            ViewState.IsLoading = false;
            ViewState.ErrorMessage = null;

            ViewState.RaiseChanged();
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            ClearError(() => ViewState.Pin);

            if (ViewState.Step == 0)
            {
                if (fieldIdentifier.FieldEquals(() => ViewState.Pin))
                {
                    if (string.IsNullOrEmpty(ViewState.Pin))
                    {
                        AddError(() => ViewState.Pin, _localizationService.GetString("GIZ_GEN_VE_REQUIRED_FIELD"));
                    }
                }
            }
        }
    }
}
