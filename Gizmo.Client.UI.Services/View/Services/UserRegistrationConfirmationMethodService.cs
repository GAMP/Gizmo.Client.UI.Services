using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationConfirmationMethodService : ValidatingViewStateServiceBase<UserRegistrationConfirmationMethodViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationConfirmationMethodService(UserRegistrationConfirmationMethodViewState viewState,
            ILogger<UserRegistrationConfirmationMethodService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            UserRegistrationViewState userRegistrationViewState,
             UserVerificationService userVerificationService,
            UserVerificationFallbackService userVerificationFallbackService) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;

            _gizmoClient = gizmoClient;
            _userRegistrationViewState = userRegistrationViewState;
            _userVerificationService = userVerificationService;
            _userVerificationFallbackService = userVerificationFallbackService;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly UserRegistrationViewState _userRegistrationViewState;
        private readonly UserVerificationService _userVerificationService;
        private readonly UserVerificationFallbackService _userVerificationFallbackService;
        #endregion

        #region FUNCTIONS

        public void SetEmail(string value)
        {
            ViewState.Email = value;
            ViewState.RaiseChanged();
        }

        public void SetCountry(string value)
        {
            ViewState.Country = value;
            ViewState.RaiseChanged();
        }

        public void SetPrefix(string value)
        {
            ViewState.Prefix = value;
            ViewState.RaiseChanged();
        }

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ViewState.RaiseChanged();
        }

        public void Clear()
        {
            ViewState.Email = string.Empty;
            ViewState.Country = string.Empty;
            ViewState.Prefix = string.Empty;
            ViewState.MobilePhone = string.Empty;
        }

        public async Task SubmitAsync(bool fallback = false)
        {
            _userVerificationService.Lock();

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            ViewState.IsValid = EditContext.Validate(); //TODO: AAA VALIDATE ASYNC?

            if (ViewState.IsValid != true)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

                _userVerificationService.Unlock();

                return;
            }

            bool wasSuccessful = false;

            try
            {
                if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.Email)
                {
                    var result = await _gizmoClient.UserCreateByEmailStartAsync(ViewState.Email);

                    if (result.Result == Gizmo.VerificationStartResultCode.Success)
                    {
                        string email = "";

                        if (!string.IsNullOrEmpty(result.Email))
                        {
                            int atIndex = result.Email.IndexOf('@');
                            if (atIndex != -1 && atIndex > 1)
                                email = result.Email.Substring(atIndex - 2).PadLeft(result.Email.Length, '*');
                            else
                                email = result.Email;
                        }

                        ViewState.Token = result.Token;
                        ViewState.Destination = email;
                        ViewState.CodeLength = result.CodeLength;

                        NavigationService.NavigateTo(ClientRoutes.RegistrationConfirmationRoute);
                    }
                    else if (result.Result == Gizmo.VerificationStartResultCode.NoRouteForDelivery)
                    {
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString("PROVIDER_NO_ROUTE_FOR_DELIVERY");
                    }
                    else
                    {
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = result.Result.ToString();
                    }
                }
                else if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone)
                {
                    var result = await _gizmoClient.UserCreateByMobileStartAsync(ViewState.MobilePhone, !fallback ? Gizmo.ConfirmationCodeDeliveryMethod.Undetermined : Gizmo.ConfirmationCodeDeliveryMethod.SMS);

                    switch (result.Result)
                    {
                        case Gizmo.VerificationStartResultCode.Success:

                            string mobile = result.MobilePhone;

                            if (mobile.Length > 4)
                                mobile = result.MobilePhone.Substring(result.MobilePhone.Length - 4).PadLeft(10, '*');

                            bool isFlashCall = result.DeliveryMethod == Gizmo.ConfirmationCodeDeliveryMethod.FlashCall;

                            if (isFlashCall)
                            {
                                _userVerificationFallbackService.SetSMSFallbackAvailability(true);
                                _userVerificationFallbackService.Lock();
                                _userVerificationFallbackService.StartUnlockTimer();
                            }
                            else
                            {
                                _userVerificationFallbackService.SetSMSFallbackAvailability(false);
                            }

                            ViewState.Token = result.Token;
                            ViewState.Destination = mobile;
                            ViewState.CodeLength = result.CodeLength;
                            ViewState.DeliveryMethod = result.DeliveryMethod;

                            NavigationService.NavigateTo(ClientRoutes.RegistrationConfirmationRoute);

                            break;

                        case Gizmo.VerificationStartResultCode.NonUniqueInput:

                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("VE_MOBILE_PHONE_USED");

                            break;

                        case Gizmo.VerificationStartResultCode.NoRouteForDelivery:

                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("PROVIDER_NO_ROUTE_FOR_DELIVERY");

                            break;

                        default:

                            ViewState.HasError = true;
                            ViewState.ErrorMessage = result.Result.ToString();

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User create start error.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = ex.ToString();
            }
            finally
            {
                ViewState.IsLoading = false;

                if (wasSuccessful)
                    _userVerificationService.StartUnlockTimer();
                else
                    _userVerificationService.Unlock();

                ViewState.RaiseChanged();
            }
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnCustomValidationAsync(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            await base.OnCustomValidationAsync(fieldIdentifier, validationMessageStore);

            if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.Email &&
                fieldIdentifier.FieldName == nameof(ViewState.Email))
            {
                if (string.IsNullOrEmpty(ViewState.Email))
                {
                    validationMessageStore.Add(() => ViewState.Email, _localizationService.GetString("EMAIL_IS_REQUIRED"));
                }
                else
                {
                    try
                    {
                        if (await _gizmoClient.UserEmailExistAsync(ViewState.Email))
                        {
                            validationMessageStore.Add(() => ViewState.Email, _localizationService.GetString("VE_EMAIL_ADDRESS_USED"));
                        }
                    }
                    catch (Exception ex)
                    {
                        validationMessageStore.Add(() => ViewState.Email, "Cannot validate email!"); //TODO: AAA TRANSLATE
                    }
                }
            }

            if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone &&
                fieldIdentifier.FieldName == nameof(ViewState.MobilePhone))
            {
                if (string.IsNullOrEmpty(ViewState.MobilePhone))
                {
                    validationMessageStore.Add(() => ViewState.MobilePhone, _localizationService.GetString("PHONE_IS_REQUIRED"));
                }
                else
                {
                    try
                    {
                        if (await _gizmoClient.UserMobileExistAsync(ViewState.MobilePhone))
                        {
                            validationMessageStore.Add(() => ViewState.MobilePhone, _localizationService.GetString("VE_MOBILE_PHONE_USED"));
                        }
                    }
                    catch (Exception ex)
                    {
                        validationMessageStore.Add(() => ViewState.MobilePhone, "Cannot validate phone!"); //TODO: AAA TRANSLATE
                    }
                }
            }
        }

        #endregion
    }
}
