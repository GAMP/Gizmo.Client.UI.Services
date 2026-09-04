using Gizmo.Client.Options;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserOnlineDepositViewService : ValidatingViewStateServiceBase<UserOnlineDepositViewState>
    {
        #region CONSTRUCTOR
        public UserOnlineDepositViewService(UserOnlineDepositViewState viewState,
            ILogger<UserOnlineDepositViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            PaymentMethodViewStateLookupService paymentMethodViewStateLookupService,
            IOptions<UserOnlineDepositOptions> userOnlineDepositOptions) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _paymentMethodViewStateLookupService = paymentMethodViewStateLookupService;
            _userOnlineDepositOptions = userOnlineDepositOptions;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly PaymentMethodViewStateLookupService _paymentMethodViewStateLookupService;
        private readonly IOptions<UserOnlineDepositOptions> _userOnlineDepositOptions;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public void SetAmount(decimal? value)
        {
            if (!value.HasValue)
            {
                ViewState.Amount = null;
                ValidateProperty(() => ViewState.Amount);
                return;
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

        public async Task SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                var result = await _gizmoClient.PaymentIntentCreateAsync(new PaymentIntentCreateParametersDepositModel()
                {
                    Amount = ViewState.Amount.Value,
                    PaymentMethodId = ViewState.SelectedPaymentMethodId.Value
                });

                ViewState.PaymentUrl = result.PaymentUrl;

                byte[] binaryQrImage = null;

                if (!string.IsNullOrEmpty(result.NativeQrImage))
                    binaryQrImage = Convert.FromBase64String(result.NativeQrImage);
                else if (!string.IsNullOrEmpty(result.QrImage))
                    binaryQrImage = Convert.FromBase64String(result.QrImage.Substring(41)); //Remove signature.

                if (binaryQrImage != null)
                {
                    ViewState.QrImage = System.Text.Encoding.ASCII.GetString(binaryQrImage);
                }

                ViewState.IsLoading = false;

                ViewState.PageIndex = 1;
                ViewState.RaiseChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Payment intent create error.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public void Clear()
        {
            ViewState.PageIndex = 0;
            ViewState.Amount = null;
            ViewState.PaymentUrl = string.Empty;
            ViewState.QrImage = string.Empty;
            ViewState.RaiseChanged();
        }

        #endregion

        #region OVERRIDES

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.LoginStateChange -= OnLoginStateChange;
            base.OnDisposing(isDisposing);
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.Amount))
            {
                //the configured limits bound a custom amount only, a preset is an amount the operator published and
                //stands on its own even when it falls outside them. the required attribute covers an empty amount.
                if (!ViewState.Amount.HasValue || ViewState.Presets.Contains(ViewState.Amount.Value))
                    return;

                //the local maximum caps a manually entered deposit regardless of the configuration, so the one that
                //binds is the lower of the two.
                var maximumAmount = ViewState.MaximumAmount.HasValue
                    ? Math.Min(ViewState.MaximumAmount.Value, _userOnlineDepositOptions.Value.MaximumAmount)
                    : _userOnlineDepositOptions.Value.MaximumAmount;

                if (ViewState.MinimumAmount.HasValue && ViewState.Amount < ViewState.MinimumAmount.Value)
                {
                    AddError(() => ViewState.Amount, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_ONLINE_DEPOSIT_MINIMUM_AMOUNT_IS), ViewState.MinimumAmount.Value));
                }
                else if (ViewState.Amount > maximumAmount)
                {
                    AddError(() => ViewState.Amount, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_ONLINE_DEPOSIT_MAXIMUM_AMOUNT_IS), maximumAmount));
                }
            }
        }

        #endregion

        private async void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            if (e.State == LoginState.LoggedIn)
            {
                try
                {
                    Clear();

                    if (!_userOnlineDepositOptions.Value.Disabled)
                    {
                        var paymentMethods = await _paymentMethodViewStateLookupService.GetStatesAsync();

                        ViewState.SelectedPaymentMethodId = paymentMethods
                            .Where(a => a.IsEnabled)
                            .Where(a => a.IsOnline)
                            .Select(a => (int?)a.Id)
                            .FirstOrDefault();

                        if (ViewState.SelectedPaymentMethodId.HasValue)
                        {
                            ViewState.IsEnabled = true;

                            var configuration = await _gizmoClient.OnlinePaymentsConfigurationGetAsync();

                            ViewState.Presets = configuration.Presets;
                            ViewState.AllowCustomValue = configuration.AllowCustomValue;
                            ViewState.MinimumAmount = configuration.MinimumAmount;
                            ViewState.MaximumAmount = configuration.MaximumAmount;
                        }
                    }

                    ViewState.RaiseChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to obtain user online deposit configuration.");
                }
            }
        }
    }
}
