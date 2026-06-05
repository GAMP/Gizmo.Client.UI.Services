using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public sealed class PasswordRecoveryViewService : ValidatingViewStateServiceBase<PasswordRecoveryViewState>
    {
        private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly IPasswordRecoverySessionService _session;
        private readonly ILocalizationService _localizationService;
        private readonly IPhoneValidationService _phoneValidationService;

        public PasswordRecoveryViewService(
            PasswordRecoveryViewState viewState,
            ILogger<PasswordRecoveryViewService> logger,
            IServiceProvider serviceProvider,
            IPasswordRecoveryService passwordRecoveryService,
            IPasswordRecoverySessionService session,
            ILocalizationService localizationService,
            IPhoneValidationService phoneValidationService) : base(viewState, logger, serviceProvider)
        {
            _passwordRecoveryService = passwordRecoveryService;
            _session = session;
            _localizationService = localizationService;
            _phoneValidationService = phoneValidationService;
        }

        public void SetMatchValue(string value)
        {
            ViewState.MatchValue = value;
            ValidateProperty(() => ViewState.MatchValue);
        }

        public void SetCountry(string? value)
        {
            ViewState.Country = value;
            ValidateProperty(() => ViewState.Country);
        }

        public void SetRegionCode(string? value)
        {
            ViewState.RegionCode = value;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void SetMobilePhone(string? value)
        {
            ViewState.MobilePhone = value;
            ViewState.PhoneE164 = null;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public async Task SubmitAsync()
        {
            if (ViewState.IsLoading)
                return;

            ViewState.IsLoading = true;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
                return;
            }

            try
            {
                var provider = _session.ActiveProvider;
                if (provider is null)
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RECOVERY_PASSWORD_RESET_FAILED_MESSAGE));
                    return;
                }

                var matchValue = GetNormalizedMatchValue();

                var result = await _passwordRecoveryService.StartAsync(new PasswordRecoveryStartRequest
                {
                    IntegrationPublicId = provider.PublicId,
                    Channel = provider.Channel,
                    MatchValue = matchValue
                });

                switch (result.Result)
                {
                    case PasswordRecoveryStartCode.Success:
                        _session.SetMatchValue(matchValue);
                        _session.SetStartResult(
                            result.Token ?? string.Empty,
                            result.Destination ?? string.Empty,
                            result.CodeLength,
                            result.ExpiresInSeconds);
                        NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryConfirmationRoute);
                        break;

                    case PasswordRecoveryStartCode.NoRouteForDelivery:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CONFIRMATION_ERROR_PROVIDER_NO_ROUTE_FOR_DELIVERY));
                        break;

                    default:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RECOVERY_PASSWORD_RESET_FAILED_MESSAGE));
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Password recovery start error.");
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            var provider = _session.ActiveProvider;

            if (provider is null)
            {
                try
                {
                    var providers = await _passwordRecoveryService.GetProvidersAsync(cancellationToken);
                    if (providers.Count == 1)
                        provider = providers[0];
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Password recovery providers load error on navigate-in.");
                }

                if (provider is null)
                {
                    NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                    return;
                }
            }

            _session.Clear();
            _session.SetActiveProvider(provider);

            ViewState.Channel = provider.Channel;
            ViewState.MatchValue = string.Empty;
            ViewState.Country = null;
            ViewState.RegionCode = null;
            ViewState.MobilePhone = null;
            ViewState.PhoneE164 = null;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ResetValidationState();
            ViewState.RaiseChanged();
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (ViewState.Channel == PasswordRecoveryChannel.Email)
            {
                if (fieldIdentifier.FieldEquals(() => ViewState.MatchValue))
                {
                    if (string.IsNullOrEmpty(ViewState.MatchValue))
                        AddError(() => ViewState.MatchValue, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                    else
                        ClearError(() => ViewState.MatchValue);
                }
            }
            else if (ViewState.Channel == PasswordRecoveryChannel.Sms)
            {
                if (fieldIdentifier.FieldEquals(() => ViewState.Country))
                {
                    if (string.IsNullOrEmpty(ViewState.Country))
                        AddError(() => ViewState.Country, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                    else
                        ClearError(() => ViewState.Country);
                }

                if (fieldIdentifier.FieldEquals(() => ViewState.RegionCode))
                {
                    if (string.IsNullOrEmpty(ViewState.RegionCode))
                        AddError(() => ViewState.RegionCode, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                    else
                        ClearError(() => ViewState.RegionCode);
                }
            }
        }

        protected override async Task<IEnumerable<string>> OnValidateAsync(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger, CancellationToken cancellationToken = default)
        {
            if (ViewState.Channel == PasswordRecoveryChannel.Sms &&
                fieldIdentifier.FieldEquals(() => ViewState.MobilePhone) &&
                !string.IsNullOrEmpty(ViewState.MobilePhone))
            {
                if (string.IsNullOrEmpty(ViewState.RegionCode))
                    return new string[] { _localizationService.GetString("GIZ_REGISTRATION_VE_SELECT_COUNTRY") };

                var formatResult = await _phoneValidationService.ValidateAsync(ViewState.MobilePhone, ViewState.RegionCode, cancellationToken);
                if (!formatResult.IsValid)
                    return new string[] { _localizationService.GetString(formatResult.ErrorKey ?? nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED)) };

                ViewState.PhoneE164 = formatResult.E164;
                return Array.Empty<string>();
            }

            return await base.OnValidateAsync(fieldIdentifier, validationTrigger, cancellationToken);
        }

        protected override AsyncValidatedDetermineResult OnDetermineIsAsyncPropertiesValidated()
        {
            if (ViewState.Channel == PasswordRecoveryChannel.Sms)
            {
                if (IsAsyncValidated(() => ViewState.MobilePhone))
                    return AsyncValidatedDetermineResult.DefaultTrue;
                return base.OnDetermineIsAsyncPropertiesValidated();
            }
            return AsyncValidatedDetermineResult.DefaultTrue;
        }

        private string GetNormalizedMatchValue()
        {
            if (_session.ActiveProvider?.Channel == PasswordRecoveryChannel.Sms)
            {
                var e164 = ViewState.PhoneE164;
                if (!string.IsNullOrEmpty(e164))
                    return e164.StartsWith("+") ? e164[1..] : e164;
            }
            return ViewState.MatchValue;
        }
    }
}
