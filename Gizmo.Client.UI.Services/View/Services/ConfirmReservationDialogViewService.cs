using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ConfirmReservationDialogViewService : ValidatingViewStateServiceBase<ConfirmReservationDialogViewState>
    {
        #region CONSTRUCTOR
        public ConfirmReservationDialogViewService(ConfirmReservationDialogViewState viewState,
            ILogger<ConfirmReservationDialogViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService,
            IClientDialogService dialogService,
            PaymentMethodViewStateLookupService paymentMethodViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _dialogService = dialogService;
            _paymentMethodViewStateLookupService = paymentMethodViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        private readonly IClientDialogService _dialogService;
        private readonly PaymentMethodViewStateLookupService _paymentMethodViewStateLookupService;

        private AddDialogResult<EmptyComponentResult>? _confirmReservationDialog = null;
        #endregion

        #region FUNCTIONS

        public void SetPin(string value)
        {
            ViewState.ErrorMessage = null;
            ViewState.Pin = value;
            ValidateProperty(() => ViewState.Pin);
        }

        public void SetPaymentMethodId(int? value)
        {
            ViewState.ErrorMessage = null;
            ViewState.PaymentMethodId = value;
            ValidateProperty(() => ViewState.PaymentMethodId);

            ViewState.SelectedPaymentMethod = ViewState.AvailablePaymentMethods.Where(a => a.Id == value).FirstOrDefault();
            ViewState.RaiseChanged();
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
                        //If confirmed and paid close the dialog.
                        _confirmReservationDialog?.Controller?.Result(new EmptyComponentResult());
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

        public async Task PayAsync()
        {
            ViewState.ErrorMessage = null;

            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                var result = await _gizmoClient.ReservationCurrentPaymentsAsync(new ClientReservationPaymentsCreateModel()
                {
                    PaymentMethodId = ViewState.PaymentMethodId.Value
                });

                if (result.Result == ClientReservationCreateResult.Success)
                {
                    if (result.ExpectedPayment == null)
                    {
                        var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewService>();
                        hostReservationViewService.ResetIgnore();

                        ViewState.Step = 2;
                    }
                    else
                    {
                        ViewState.HasQr = true;

                        var qrResult = await _gizmoClient.GenerateQRCodeFromUrlAsync(result.ExpectedPayment.PaymentUrl);
                        ViewState.QRCode = qrResult.QRCode;
                    }
                }
                else
                {
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

        public void CloseIfWaitingPayment()
        {
            //Close dialog if waiting payment.
            if (_confirmReservationDialog != null && ViewState.Step == 1)
            {
                var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewService>();
                hostReservationViewService.ResetIgnore();

                ViewState.Step = 2;
                ViewState.RaiseChanged();
            }
        }

        public void Ignore()
        {
            var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewService>();
            hostReservationViewService.Ignore();
            _confirmReservationDialog?.Controller?.Result(new EmptyComponentResult());
        }

        public void Close()
        {
            var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewService>();
            hostReservationViewService.ResetIgnore();
            _confirmReservationDialog?.Controller?.Result(new EmptyComponentResult());
        }

        public async Task StartAsync(CancellationToken cToken = default)
        {
            if (_confirmReservationDialog != null)
                return;

            Clear();

            try
            {
                var result = await _gizmoClient.ReservationCurrentConfirmedAsync(cToken);

                if (result == ReservationCurrentConfirmedResult.Confirmed)
                {
                    var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewService>();

                    if (hostReservationViewService.ViewState.ReservationPaymentStatus == Web.Api.Models.ReservationPaymentStatus.NotRequired ||
                        hostReservationViewService.ViewState.ReservationPaymentStatus == Web.Api.Models.ReservationPaymentStatus.Satisfied)
                    {
                        //If confirmed and paid do no show the dialog.
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

                var paymentMethods = await _paymentMethodViewStateLookupService.GetStatesAsync(cToken);
                ViewState.AvailablePaymentMethods = paymentMethods.Where(a => (a.Id > 0 || a.Id == -3) && !a.IsDeleted && a.IsEnabled).ToList();

                _confirmReservationDialog = await _dialogService.ShowConfirmReservationDialogAsync(cToken);
                if (_confirmReservationDialog.Result == AddComponentResultCode.Opened)
                    await _confirmReservationDialog.WaitForResultAsync(cToken);

                _confirmReservationDialog = null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to show confirm reservation dialog.");
            }
        }

        public void Clear()
        {
            ViewState.AvailablePaymentMethods = [];
            ViewState.Step = 0;
            ViewState.Pin = null;
            ViewState.PaymentMethodId = null;
            ViewState.SelectedPaymentMethod = null;
            ViewState.IsLoading = false;
            ViewState.HasQr = false;
            ViewState.QRCode = null;

            ViewState.RaiseChanged();
        }

        #endregion

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            ClearError(() => ViewState.Pin);
            ClearError(() => ViewState.PaymentMethodId);

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
            else if (ViewState.Step == 1)
            {
                if (fieldIdentifier.FieldEquals(() => ViewState.PaymentMethodId))
                {
                    if (!ViewState.PaymentMethodId.HasValue)
                    {
                        AddError(() => ViewState.PaymentMethodId, _localizationService.GetString("GIZ_GEN_VE_REQUIRED_FIELD"));
                    }
                }
            }
        }
    }
}
