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
    public sealed class UserRegistrationConfirmationMethodViewService : ValidatingViewStateServiceBase<UserRegistrationConfirmationMethodViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationConfirmationMethodViewService(UserRegistrationConfirmationMethodViewState viewState,
            ILogger<UserRegistrationConfirmationMethodViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            UserRegistrationViewState userRegistrationViewState,
             UserVerificationViewService userVerificationService,
            UserVerificationFallbackViewService userVerificationFallbackService) : base(viewState, logger, serviceProvider)
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
        private readonly UserVerificationViewService _userVerificationService;
        private readonly UserVerificationFallbackViewService _userVerificationFallbackService;
        #endregion

        #region FUNCTIONS

        public void SetEmail(string value)
        {
            ViewState.Email = value;
            ValidateProperty(() => ViewState.Email);
        }

        public void SetCountry(string value)
        {
            ViewState.Country = value;
            ValidateProperty(() => ViewState.Country);
        }

        //public void SetPrefix(string value)
        //{
        //    ViewState.Prefix = value;
        //    DebounceViewStateChanged();
        //}

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void Clear()
        {
            ViewState.Email = string.Empty;
            ViewState.Country = string.Empty;
            //ViewState.Prefix = string.Empty;
            ViewState.MobilePhone = string.Empty;

            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ResetValidationState();
            DebounceViewStateChanged();
        }

        public async Task SubmitAsync(bool fallback = false)
        {
            if (await _userVerificationService.LockAsync())
            {
                ViewState.IsLoading = true;
                ViewState.RaiseChanged();

                Validate();

                if (ViewState.IsValid != true)
                {
                    ViewState.IsLoading = false;
                    ViewState.RaiseChanged();

                    await _userVerificationService.Unlock();

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
                            ViewState.ErrorMessage = _localizationService.GetString("GIZ_USER_CONFIRMATION_ERROR_PROVIDER_NO_ROUTE_FOR_DELIVERY");
                        }
                        else
                        {
                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED") + $" {result.Result.ToString()}"; //TODO: AAA TRANSLATE?
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
                                ViewState.ErrorMessage = _localizationService.GetString("GIZ_REGISTRATION_VE_MOBILE_PHONE_USED");

                                break;

                            case Gizmo.VerificationStartResultCode.NoRouteForDelivery:

                                ViewState.HasError = true;
                                ViewState.ErrorMessage = _localizationService.GetString("GIZ_USER_CONFIRMATION_ERROR_PROVIDER_NO_ROUTE_FOR_DELIVERY");

                                break;

                            default:

                                ViewState.HasError = true;
                                ViewState.ErrorMessage = _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED") + $" {result.Result.ToString()}"; //TODO: AAA TRANSLATE?

                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "User create start error.");

                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED");
                }
                finally
                {
                    ViewState.IsLoading = false;

                    if (wasSuccessful)
                        _userVerificationService.StartUnlockTimer();
                    else
                        await _userVerificationService.Unlock();

                    ViewState.RaiseChanged();
                }
            }
        }

        #endregion

        #region OVERRIDES

        //protected override void OnValidationStateChanged()
        //{
        //    IsAsyncValidating(() => ViewState.MobilePhone);

        //    base.OnValidationStateChanged();
        //}

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.Email &&
                fieldIdentifier.FieldEquals(() => ViewState.Email))
            {
                if (string.IsNullOrEmpty(ViewState.Email))
                {
                    AddError(() => ViewState.Email, _localizationService.GetString("GIZ_USER_CONFIRMATION_VE_EMAIL_IS_REQUIRED"));
                }
            }

            if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone &&
                fieldIdentifier.FieldEquals(() => ViewState.MobilePhone))
            {
                if (string.IsNullOrEmpty(ViewState.MobilePhone))
                {
                    AddError(() => ViewState.MobilePhone, _localizationService.GetString("GIZ_USER_CONFIRMATION_VE_PHONE_IS_REQUIRED"));
                }
            }
        }

        protected override async Task<IEnumerable<string>> OnValidateAsync(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger, CancellationToken cancellationToken = default)
        {
            if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.Email &&
                fieldIdentifier.FieldEquals(() => ViewState.Email))
            {
                if (!string.IsNullOrEmpty(ViewState.Email))
                {
                    try
                    {
                        if (await _gizmoClient.UserEmailExistAsync(ViewState.Email))
                        {
                            return new string[] { _localizationService.GetString("GIZ_REGISTRATION_VE_EMAIL_ADDRESS_USED") };
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Cannot validate email.");
                        return new string[] { _localizationService.GetString("GIZ_REGISTRATION_VE_CANNOT_VALIDATE_EMAIL") };
                    }
                }
            }

            if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone &&
                fieldIdentifier.FieldEquals(() => ViewState.MobilePhone))
            {
                if (!string.IsNullOrEmpty(ViewState.MobilePhone))
                {
                    try
                    {
                        //var tmp = ViewState.Prefix + ViewState.MobilePhone;
                        var tmp = ViewState.MobilePhone;
                        if (tmp.StartsWith("+"))
                        {
                            tmp = tmp.Substring(1);
                        }

                        if (await _gizmoClient.UserMobileExistAsync(tmp))
                        {
                            return new string[] { _localizationService.GetString("GIZ_REGISTRATION_VE_MOBILE_PHONE_USED") };
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Cannot validate phone.");
                        return new string[] { _localizationService.GetString("GIZ_REGISTRATION_VE_CANNOT_VALIDATE_PHONE") };
                    }
                }
            }

            return await base.OnValidateAsync(fieldIdentifier, validationTrigger, cancellationToken);
        }

        protected override AsyncValidatedDetermineResult OnDetermineIsAsyncPropertiesValidated()
        {
            switch(_userRegistrationViewState.ConfirmationMethod)
            {
                case RegistrationVerificationMethod.MobilePhone:
                    if (IsAsyncValidated(() => ViewState.MobilePhone))
                        return AsyncValidatedDetermineResult.DefaultTrue;
                    break;
                case RegistrationVerificationMethod.Email:
                    if (IsAsyncValidated(() => ViewState.Email))
                        return AsyncValidatedDetermineResult.DefaultTrue;
                    break;
                default:
                    break;
            }

            return base.OnDetermineIsAsyncPropertiesValidated();
        }

        #endregion

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
        }
    }
}
