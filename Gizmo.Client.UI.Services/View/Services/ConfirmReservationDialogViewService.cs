using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
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
            IClientDialogService dialogService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _dialogService = dialogService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        private readonly IClientDialogService _dialogService;

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
                //TODO: AAAAA SUBMIT
                await Task.Delay(5000);

                ViewState.Step = 1;
            }
            catch (Exception ex)
            {
            }

            ViewState.IsLoading = false;
            ViewState.RaiseChanged();
        }

        public async Task StartAsync(CancellationToken cToken = default)
        {
            if (_confirmReservationDialog != null)
                return;

            Clear();

            var hostReservationViewService = ServiceProvider.GetRequiredService<HostReservationViewService>();

            if (hostReservationViewService.ViewState.IsConfirmed)
            {
                if (hostReservationViewService.ViewState.ReservationPaymentStatus == Web.Api.Models.ReservationPaymentStatus.NotRequired ||
                    hostReservationViewService.ViewState.ReservationPaymentStatus == Web.Api.Models.ReservationPaymentStatus.Satisfied)
                {
                    return;
                }
                else
                {
                    ViewState.Step = 1;
                }
            }

            try
            {
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
            ViewState.Step = 0;
            ViewState.Pin = null;
            ViewState.PaymentMethodId = null;
            ViewState.IsLoading = false;

            ViewState.RaiseChanged();
        }

        #endregion
    }
}
