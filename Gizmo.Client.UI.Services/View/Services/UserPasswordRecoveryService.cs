using Gizmo.Client.UI.View.States;
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
    public sealed class UserPasswordRecoveryService : ValidatingViewStateServiceBase<UserPasswordRecoveryViewState>
    {
        #region CONTRUCTOR
        public UserPasswordRecoveryService(UserPasswordRecoveryViewState viewState,
            ILogger<UserPasswordRecoveryService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            UserPasswordRecoveryMethodServiceViewState userPasswordRecoveryMethodServiceViewState) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _userPasswordRecoveryMethodServiceViewState = userPasswordRecoveryMethodServiceViewState;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly UserPasswordRecoveryMethodServiceViewState _userPasswordRecoveryMethodServiceViewState;
        #endregion

        #region FUNCTIONS

        public void SetEmail(string value)
        {
            ViewState.Email = value;
            ViewState.RaiseChanged();
        }

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ViewState.RaiseChanged();
        }

        public void SetSelectedRecoveryMethod(UserRecoveryMethod value)
        {
            //Do not allow the user to change the recovery method, use the recovery method specified on configuration.
            if (!(_userPasswordRecoveryMethodServiceViewState.AvailabledRecoveryMethod.HasFlag(UserRecoveryMethod.Mobile) && _userPasswordRecoveryMethodServiceViewState.AvailabledRecoveryMethod.HasFlag(UserRecoveryMethod.Email)))
                return;

            ViewState.SelectedRecoveryMethod = value;

            ViewState.RaiseChanged();
        }

        public async Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

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

                            //TODO: AAA ViewState.CanResend = false;

                            NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryConfirmationRoute);

                            break;

                        case PasswordRecoveryStartResultCode.NoRouteForDelivery:
                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("PROVIDER_NO_ROUTE_FOR_DELIVERY");

                            break;

                        default:

                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("PASSWORD_RESET_FAILED_MESSAGE");

                            break;
                    }
                }
                else
                {
                    var result = await _gizmoClient.UserPasswordRecoveryByMobileStartAsync(ViewState.MobilePhone);

                    if (result.Result != PasswordRecoveryStartResultCode.Success)
                    {
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = result.Result.ToString(); //TODO: AAA ERROR

                        return;
                    }

                    ViewState.Token = result.Token;
                    ViewState.Destination = result.MobilePhone;
                    ViewState.CodeLength = result.CodeLength;
                    //TODO: AAA ViewState.CanResend = false;

                    NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryConfirmationRoute);
                }
            }
            catch (Exception ex)
            {
                ViewState.HasError = true;
                ViewState.ErrorMessage = ex.ToString();
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (_userPasswordRecoveryMethodServiceViewState.AvailabledRecoveryMethod.HasFlag(UserRecoveryMethod.Mobile) && _userPasswordRecoveryMethodServiceViewState.AvailabledRecoveryMethod.HasFlag(UserRecoveryMethod.Email))
                ViewState.SelectedRecoveryMethod = UserRecoveryMethod.Mobile;
            else
                ViewState.SelectedRecoveryMethod = _userPasswordRecoveryMethodServiceViewState.AvailabledRecoveryMethod;

            return Task.CompletedTask;
        }

        protected override void OnCustomValidation(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            base.OnCustomValidation(fieldIdentifier, validationMessageStore);

            if (ViewState.SelectedRecoveryMethod == UserRecoveryMethod.Email &&
                fieldIdentifier.FieldName == nameof(ViewState.Email) &&
                string.IsNullOrEmpty(ViewState.Email))
            {
                validationMessageStore.Add(() => ViewState.Email, _localizationService.GetString("EMAIL_IS_REQUIRED"));
            }


            if (ViewState.SelectedRecoveryMethod == UserRecoveryMethod.Mobile &&
                fieldIdentifier.FieldName == nameof(ViewState.MobilePhone) &&
                string.IsNullOrEmpty(ViewState.MobilePhone))
            {
                validationMessageStore.Add(() => ViewState.MobilePhone, _localizationService.GetString("PHONE_IS_REQUIRED"));
            }
        }

        #endregion
    }
}
