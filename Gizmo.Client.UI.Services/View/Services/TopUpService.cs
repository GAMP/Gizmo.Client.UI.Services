using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class TopUpService : ValidatingViewStateServiceBase<TopUpViewState>
    {
        #region CONSTRUCTOR
        public TopUpService(TopUpViewState viewState,
            ILogger<TopUpService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            IClientDialogService dialogService) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _dialogService = dialogService;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly IClientDialogService _dialogService;
        private CancellationTokenSource? _dialogCancellationTokenSource = null;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public void SetAmount(decimal value)
        {
            if (!ViewState.AllowCustomValue && !ViewState.Presets.Contains(value))
                return;

            ViewState.Amount = value;
            ValidateProperty(() => ViewState.Amount);
            DebounceViewStateChanged();
        }

        public async Task ShowDialogAsync()
        {
            var configuration = await _gizmoClient.OnlinePaymentsConfigurationGetAsync();

            ViewState.Presets = configuration.Presets;
            ViewState.AllowCustomValue = configuration.AllowCustomValue;
            ViewState.MinimumAmount = configuration.MinimumAmount;
            ViewState.RaiseChanged();

            if (_dialogCancellationTokenSource != null)
            {
                _dialogCancellationTokenSource.Dispose();
            }

            _dialogCancellationTokenSource = new CancellationTokenSource();

            var s = await _dialogService.ShowTopUpDialogAsync(_dialogCancellationTokenSource.Token);
            if (s.Result == DialogAddResult.Success)
            {
                try
                {
                    var result = await s.WaitForDialogResultAsync();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        public void SelectPreset(decimal amount)
        {
            if (ViewState.Presets.Contains(amount))
            {
                ViewState.Amount = amount;
                DebounceViewStateChanged();
            }
        }

        public async Task SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                // Simulate task.
                await Task.Delay(2000);

                ViewState.IsLoading = false;

                ViewState.PageIndex = 1;
                ViewState.RaiseChanged();
            }
            catch
            {

            }
            finally
            {

            }
        }

        public async Task PayFromPC()
        {
            _dialogCancellationTokenSource?.Cancel();

            ViewState.PageIndex = 0;
            ViewState.RaiseChanged();


            var s = await _dialogService.ShowPaymentDialogAsync();
            if (s.Result == DialogAddResult.Success)
            {
                try
                {
                    var result = await s.WaitForDialogResultAsync();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        #endregion

        #region OVERRIDES

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.Amount))
            {
                if (ViewState.Amount < ViewState.MinimumAmount)
                {
                    AddError(() => ViewState.Amount, _localizationService.GetString("TOP_UP_MINIMUM_AMOUNT_IS", ViewState.MinimumAmount));
                }
            }
        }

        #endregion
    }
}
