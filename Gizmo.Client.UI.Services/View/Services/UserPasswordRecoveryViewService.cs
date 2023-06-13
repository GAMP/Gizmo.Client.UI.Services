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
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public sealed class UserPasswordRecoveryViewService : ValidatingViewStateServiceBase<UserPasswordRecoveryViewState>
    {
        #region CONTRUCTOR
        public UserPasswordRecoveryViewService(UserPasswordRecoveryViewState viewState,
            ILogger<UserPasswordRecoveryViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            UserPasswordRecoveryMethodServiceViewState userPasswordRecoveryMethodServiceViewState,
             UserVerificationViewService userVerificationService,
            UserVerificationFallbackViewService userVerificationFallbackService) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _userPasswordRecoveryMethodServiceViewState = userPasswordRecoveryMethodServiceViewState;
            _userVerificationService = userVerificationService;
            _userVerificationFallbackService = userVerificationFallbackService;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly UserPasswordRecoveryMethodServiceViewState _userPasswordRecoveryMethodServiceViewState;
        private readonly UserVerificationViewService _userVerificationService;
        private readonly UserVerificationFallbackViewService _userVerificationFallbackService;
        #endregion

        #region FUNCTIONS

        public void SetEmail(string value)
        {
            ViewState.Email = value;
            ValidateProperty(() => ViewState.Email);
        }

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void SetSelectedRecoveryMethod(UserRecoveryMethod value)
        {
            //Do not allow the user to change the recovery method, use the recovery method specified on configuration.
            if (!(_userPasswordRecoveryMethodServiceViewState.AvailabledRecoveryMethod.HasFlag(UserRecoveryMethod.Mobile) && _userPasswordRecoveryMethodServiceViewState.AvailabledRecoveryMethod.HasFlag(UserRecoveryMethod.Email)))
                return;

            ViewState.SelectedRecoveryMethod = value;

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
                if (ViewState.SelectedRecoveryMethod == UserRecoveryMethod.Email)
                {
                    var result = await _gizmoClient.UserPasswordRecoveryByEmailStartAsync(ViewState.Email);

                    switch (result.Result)
                    {
                        case PasswordRecoveryStartResultCode.Success:

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

                            wasSuccessful = true;

                            NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryConfirmationRoute);

                            break;

                        case PasswordRecoveryStartResultCode.NoRouteForDelivery:
                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("GIZ_USER_CONFIRMATION_ERROR_PROVIDER_NO_ROUTE_FOR_DELIVERY");

                            break;

                        default:

                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_RECOVERY_PASSWORD_RESET_FAILED_MESSAGE");

                            break;
                    }
                }
                else
                {
                    //TODO: AAA 9digit phones?
                    var result = await _gizmoClient.UserPasswordRecoveryByMobileStartAsync(ViewState.MobilePhone, !fallback ? Gizmo.ConfirmationCodeDeliveryMethod.Undetermined : Gizmo.ConfirmationCodeDeliveryMethod.SMS);

                    switch (result.Result)
                    {
                        case PasswordRecoveryStartResultCode.Success:

                            string mobile = result.MobilePhone;

                            if (mobile.Length > 4)
                                mobile = result.MobilePhone.Substring(result.MobilePhone.Length - 4).PadLeft(10, '*');

                            bool isFlashCall = result.DeliveryMethod == ConfirmationCodeDeliveryMethod.FlashCall;
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

                            wasSuccessful = true;

                            NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryConfirmationRoute);

                            break;

                        case PasswordRecoveryStartResultCode.NoRouteForDelivery:

                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("GIZ_USER_CONFIRMATION_ERROR_PROVIDER_NO_ROUTE_FOR_DELIVERY");

                            break;

                        case PasswordRecoveryStartResultCode.Failed:
                        case PasswordRecoveryStartResultCode.DeliveryFailed:

                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_RECOVERY_PASSWORD_RESET_FAILED_MESSAGE");

                            break;

                        case PasswordRecoveryStartResultCode.InvalidInput:

                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_RECOVERY_PASSWORD_RESET_FAILED_MESSAGE");
                            ViewState.ErrorMessage += " " + _localizationService.GetString("GIZ_PASSWORD_RECOVERY_NO_VALID_MOBILE");

                            break;

                        default:

                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_RECOVERY_PASSWORD_RESET_FAILED_MESSAGE");

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Password recovery start error.");

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

        public void ClearAll()
        {
            var userPasswordRecoveryConfirmationService = ServiceProvider.GetRequiredService<UserPasswordRecoveryConfirmationViewService>();
            var userPasswordRecoverySetNewPasswordService = ServiceProvider.GetRequiredService<UserPasswordRecoverySetNewPasswordViewService>();

            userPasswordRecoveryConfirmationService.Clear();
            userPasswordRecoverySetNewPasswordService.Clear();

            ViewState.SelectedRecoveryMethod = UserRecoveryMethod.None;
            ViewState.MobilePhone = string.Empty;
            ViewState.Email = string.Empty;
            ViewState.Destination = string.Empty;
            ViewState.Token = string.Empty;
            ViewState.CodeLength = 0;
            ViewState.DeliveryMethod = ConfirmationCodeDeliveryMethod.Undetermined;
            
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ResetValidationState();
            DebounceViewStateChanged();
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ClearAll();

            if (_userPasswordRecoveryMethodServiceViewState.AvailabledRecoveryMethod.HasFlag(UserRecoveryMethod.Mobile) && _userPasswordRecoveryMethodServiceViewState.AvailabledRecoveryMethod.HasFlag(UserRecoveryMethod.Email))
                ViewState.SelectedRecoveryMethod = UserRecoveryMethod.Mobile;
            else
                ViewState.SelectedRecoveryMethod = _userPasswordRecoveryMethodServiceViewState.AvailabledRecoveryMethod;

            return Task.CompletedTask;
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (ViewState.SelectedRecoveryMethod == UserRecoveryMethod.Email &&
                fieldIdentifier.FieldEquals(() => ViewState.Email) &&
                string.IsNullOrEmpty(ViewState.Email))
            {
                AddError(() => ViewState.Email, _localizationService.GetString("GIZ_USER_CONFIRMATION_VE_EMAIL_IS_REQUIRED"));
            }

            if (ViewState.SelectedRecoveryMethod == UserRecoveryMethod.Mobile &&
                fieldIdentifier.FieldEquals(() => ViewState.MobilePhone) &&
                string.IsNullOrEmpty(ViewState.MobilePhone))
            {
                AddError(() => ViewState.MobilePhone, _localizationService.GetString("GIZ_USER_CONFIRMATION_VE_PHONE_IS_REQUIRED"));
            }
        }

        #endregion
    }
}
