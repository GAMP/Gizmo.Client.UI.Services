using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class TopUpViewStateService : ValidatingViewStateServiceBase<TopUpViewState>
    {
        #region CONSTRUCTOR
        public TopUpViewStateService(TopUpViewState viewState,
            ILogger<TopUpViewStateService> logger,
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

        public void SetAmount(decimal? value)
        {
            if (!value.HasValue)
            {
                ViewState.Amount = null;
            }

            if (!ViewState.AllowCustomValue && !ViewState.Presets.Contains(value.Value))
                return;

            ViewState.Amount = value;
            ValidateProperty(() => ViewState.Amount);
        }

        public void SelectPreset(decimal amount)
        {
            if (ViewState.Presets.Contains(amount))
            {
                ViewState.Amount = amount;
                ValidateProperty(() => ViewState.Amount);
            }
        }

        public async Task ShowDialogAsync()
        {
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

        public async Task SubmitAsync()
        {
            if (!ViewState.Amount.HasValue)
                return;

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

        public void Clear()
        {
            ViewState.PageIndex = 0;
            ViewState.Amount = null;
            ViewState.RaiseChanged();
        }

        public async Task PayFromPC()
        {
            _dialogCancellationTokenSource?.Cancel();

            ViewState.PageIndex = 0;
            ViewState.Amount = null;
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

        protected override async Task OnInitializing(CancellationToken ct)
        {
            try
            {
                var configuration = await _gizmoClient.OnlinePaymentsConfigurationGetAsync();

                ViewState.Presets = configuration.Presets;
                ViewState.AllowCustomValue = configuration.AllowCustomValue;
                ViewState.MinimumAmount = configuration.MinimumAmount;
                ViewState.RaiseChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to obtain top up configuration.");
            }

            await base.OnInitializing(ct);
        }

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
