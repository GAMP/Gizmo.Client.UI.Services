﻿using Gizmo.Client.UI.Services;
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
    public sealed class TopUpViewStateService : ValidatingViewStateServiceBase<TopUpViewState>
    {
        #region CONSTRUCTOR
        public TopUpViewStateService(TopUpViewState viewState,
            ILogger<TopUpViewStateService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            IClientDialogService dialogService,
            PaymentMethodViewStateLookupService paymentMethodViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _dialogService = dialogService;
            _paymentMethodViewStateLookupService = paymentMethodViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly IClientDialogService _dialogService;
        private readonly PaymentMethodViewStateLookupService _paymentMethodViewStateLookupService;
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

        //public async Task ShowDialogAsync()
        //{
        //    if (_dialogCancellationTokenSource != null)
        //    {
        //        _dialogCancellationTokenSource.Dispose();
        //    }

        //    _dialogCancellationTokenSource = new CancellationTokenSource();

        //    var s = await _dialogService.ShowTopUpDialogAsync(_dialogCancellationTokenSource.Token);
        //    if (s.Result == DialogAddResult.Success)
        //    {
        //        try
        //        {
        //            var result = await s.WaitForDialogResultAsync();
        //        }
        //        catch (OperationCanceledException)
        //        {
        //        }
        //    }
        //}

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
                    //TODO: AAA UserId = this.Client.CurrentUser.Id,
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
                ViewState.ErrorMessage = ex.ToString(); //TODO: AAA
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

        public async Task PayFromPC()
        {
            _dialogCancellationTokenSource?.Cancel();

            Clear();

            var s = await _dialogService.ShowPaymentDialogAsync(new PaymentDialogParameters() { Url = ViewState.PaymentUrl });
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
                if (ViewState.Amount < ViewState.MinimumAmount)
                {
                    AddError(() => ViewState.Amount, _localizationService.GetString("TOP_UP_MINIMUM_AMOUNT_IS", ViewState.MinimumAmount));
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
                    var paymentMethods = await _paymentMethodViewStateLookupService.GetStatesAsync();

                    ViewState.SelectedPaymentMethodId = paymentMethods.Where(a => a.IsOnline).Select(a => (int?)a.Id).FirstOrDefault();

                    if (ViewState.SelectedPaymentMethodId.HasValue)
                    {
                        ViewState.IsEnabled = true; //TODO: AAA

                        var configuration = await _gizmoClient.OnlinePaymentsConfigurationGetAsync();

                        ViewState.Presets = configuration.Presets;
                        ViewState.AllowCustomValue = configuration.AllowCustomValue;
                        ViewState.MinimumAmount = configuration.MinimumAmount;
                    }

                    ViewState.RaiseChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to obtain top up configuration.");
                }
            }
        }
    }
}
