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
            ViewState.Pin = value;
            ValidateProperty(() => ViewState.Pin);
        }

        public void SetPaymentMethodId(int? value)
        {
            ViewState.PaymentMethodId = value;
            ValidateProperty(() => ViewState.PaymentMethodId);

            ViewState.SelectedPaymentMethod = ViewState.AvailablePaymentMethods.Where(a => a.Id == value).FirstOrDefault();
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
                    SetPin(string.Empty);
                    //TODO: AAAAA SHOW ERROR
                }
            }
            catch (Exception ex)
            {
                //TODO: AAAAA SHOW ERROR
            }

            ViewState.IsLoading = false;
            ViewState.RaiseChanged();
        }

        public async Task PayAsync()
        {
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
                        //TODO: AAAAA CHECK
                        //var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewService>();
                        //hostReservationViewService.SetPaid();

                        ViewState.Step = 2;
                    }
                    else
                    {
                        ViewState.HasQr = true;
                    }
                }
                else
                {
                    //TODO: AAAAA SHOW ERROR
                }
            }
            catch (Exception ex)
            {
                //TODO: AAAAA SHOW ERROR
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
            ViewState.IsLoading = false;

            ViewState.RaiseChanged();
        }

        #endregion
    }
}
