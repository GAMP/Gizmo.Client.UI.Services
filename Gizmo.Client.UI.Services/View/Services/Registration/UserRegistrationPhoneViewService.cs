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
    [Register()]
    [Route(ClientRoutes.RegistrationPhoneRoute)]
    public sealed class UserRegistrationPhoneViewService : ValidatingViewStateServiceBase<UserRegistrationPhoneViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationPhoneViewService(
            UserRegistrationPhoneViewState viewState,
            ILogger<UserRegistrationPhoneViewService> logger,
            IServiceProvider serviceProvider,
            IUserRegistrationService registrationService,
            IRegistrationSessionService registrationSession,
            ILocalizationService localizationService,
            IPhoneValidationService phoneValidationService) : base(viewState, logger, serviceProvider)
        {
            _registrationService = registrationService;
            _registrationSession = registrationSession;
            _localizationService = localizationService;
            _phoneValidationService = phoneValidationService;
        }
        #endregion

        #region FIELDS
        private readonly IUserRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly ILocalizationService _localizationService;
        private readonly IPhoneValidationService _phoneValidationService;
        #endregion

        #region FUNCTIONS

        public void SetCountry(string value)
        {
            ViewState.Country = value;
            ValidateProperty(() => ViewState.Country);
        }

        public void SetRegionCode(string? value)
        {
            ViewState.RegionCode = value;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public async Task SubmitAsync()
        {
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

            var phone = _registrationSession.PhoneE164 ?? ViewState.MobilePhone ?? string.Empty;
            if (phone.StartsWith("+"))
                phone = phone.Substring(1);

            var integrationPublicId = _registrationSession.SelectedProvider?.PublicId ?? Guid.Empty;

            try
            {
                var result = await _registrationService.StartAsync(new RegistrationStartRequest
                {
                    Phone = phone,
                    DeliveryMethod = RegistrationDeliveryMethod.CodeDispatch,
                    IntegrationPublicId = integrationPublicId
                });

                switch (result.Result)
                {
                    case RegistrationStartCode.Success:
                        _registrationSession.SetStartResult(
                            result.Token ?? string.Empty,
                            result.Destination ?? string.Empty,
                            result.CodeLength,
                            result.ExpiresInSeconds,
                            RegistrationFlow.Sms);
                        _registrationSession.SetContactDetails(phone, ViewState.Country);
                        NavigationService.NavigateTo(ClientRoutes.RegistrationConfirmationRoute);
                        break;

                    case RegistrationStartCode.NonUniqueInput:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_MOBILE_PHONE_USED));
                        break;

                    case RegistrationStartCode.NoRouteForDelivery:
                    case RegistrationStartCode.DeliveryFailed:
                    case RegistrationStartCode.Failed:
                    case RegistrationStartCode.Unknown:
                        Logger.LogWarning("Registration phone start failed for provider {ProviderPublicId}: {Result}", integrationPublicId, result.Result);
                        NavigateToProvidersWithProviderFailure();
                        break;

                    default:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED)) + $" {result.Result}";
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Registration phone start error.");
                NavigateToProvidersWithProviderFailure();
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        private void NavigateToProvidersWithProviderFailure()
        {
            _registrationSession.SetFailedProviderChannelGuid(_registrationSession.SelectedProvider?.ChannelGuid ?? Guid.Empty);
            NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
        }

        #endregion

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
        }

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.Country = null;
            ViewState.RegionCode = null;
            ViewState.MobilePhone = null;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ResetValidationState();
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
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

        protected override async Task<IEnumerable<string>> OnValidateAsync(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger, CancellationToken cancellationToken = default)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.MobilePhone) && !string.IsNullOrEmpty(ViewState.MobilePhone))
            {
                if (string.IsNullOrEmpty(ViewState.RegionCode))
                    return new string[] { _localizationService.GetString("GIZ_REGISTRATION_VE_SELECT_COUNTRY") };

                var formatResult = await _phoneValidationService.ValidateAsync(ViewState.MobilePhone, ViewState.RegionCode, cancellationToken);
                if (!formatResult.IsValid)
                    return new string[] { _localizationService.GetString(formatResult.ErrorKey ?? nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED)) };

                _registrationSession.SetPhoneE164(formatResult.E164);

                try
                {
                    var phone = formatResult.E164 ?? ViewState.MobilePhone;
                    if (phone.StartsWith("+"))
                        phone = phone.Substring(1);

                    if (await _registrationService.MobilePhoneExistAsync(phone, cancellationToken))
                    {
                        return new string[] { _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_MOBILE_PHONE_USED)) };
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Cannot validate phone.");
                    return new string[] { _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_CANNOT_VALIDATE_PHONE)) };
                }
            }

            return await base.OnValidateAsync(fieldIdentifier, validationTrigger, cancellationToken);
        }

        protected override AsyncValidatedDetermineResult OnDetermineIsAsyncPropertiesValidated()
        {
            if (IsAsyncValidated(() => ViewState.MobilePhone))
                return AsyncValidatedDetermineResult.DefaultTrue;

            return base.OnDetermineIsAsyncPropertiesValidated();
        }

        #endregion
    }
}
