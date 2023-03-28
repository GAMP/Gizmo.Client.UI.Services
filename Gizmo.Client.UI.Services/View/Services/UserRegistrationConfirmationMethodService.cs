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
            ValidateProperty(() => ViewState.Email);
            DebounceViewStateChanged();
        }

        public void SetCountry(string value)
        {
            ViewState.Country = value;
            ValidateProperty(() => ViewState.Country);
            DebounceViewStateChanged();
        }

        public void SetPrefix(string value)
        {
            ViewState.Prefix = value;
            DebounceViewStateChanged();
        }

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ValidateProperty(() => ViewState.MobilePhone);
            DebounceViewStateChanged();
        }

        public void Clear()
        {
            ViewState.Email = string.Empty;
            ViewState.Country = string.Empty;
            ViewState.Prefix = string.Empty;
            ViewState.MobilePhone = string.Empty;

            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            DebounceViewStateChanged();
        }

        public async Task SubmitAsync(bool fallback = false)
        {
            try
            {
                await _userVerificationService.LockAsync();
            }
            catch
            {
                return;
            }

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            Validate();

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
                    AddError(() => ViewState.Email, _localizationService.GetString("EMAIL_IS_REQUIRED"));
                }
            }

            if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone &&
                fieldIdentifier.FieldEquals(() => ViewState.MobilePhone))
            {
                if (string.IsNullOrEmpty(ViewState.MobilePhone))
                {
                    AddError(() => ViewState.MobilePhone, _localizationService.GetString("PHONE_IS_REQUIRED"));
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
                            return new string[] { _localizationService.GetString("VE_EMAIL_ADDRESS_USED") };
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Cannot validate email.");
                        return new string[] { "Cannot validate email." }; //TODO: AAA TRANSLATE
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
                        var tmp = ViewState.Prefix + ViewState.MobilePhone;
                        if (tmp.StartsWith("+"))
                        {
                            tmp = tmp.Substring(1);
                        }

                        if (await _gizmoClient.UserMobileExistAsync(tmp))
                        {
                            return new string[] { _localizationService.GetString("VE_MOBILE_PHONE_USED") };
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Cannot validate phone.");
                        return new string[] { "Cannot validate phone." }; //TODO: AAA TRANSLATE
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
    }
}
