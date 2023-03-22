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

                    if (result.Result != PasswordRecoveryStartResultCode.Success)
                    {
                        //TODO: A HANDLE ERROR
                    }

                    ViewState.Token = result.Token;
                }
                else
                {
                    var result = await _gizmoClient.UserPasswordRecoveryByMobileStartAsync(ViewState.MobilePhone);

                    if (result.Result != PasswordRecoveryStartResultCode.Success)
                    {
                        //TODO: A HANDLE ERROR
                    }

                    ViewState.Token = result.Token;
                }

                // Simulate task.
                await Task.Delay(2000);

                ViewState.IsLoading = false;

                //ViewState.CanResend = false;

                NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryConfirmationRoute);

                //TODO: A
                //ViewState.CanResend = false;
                await Task.Delay(5000);
                ViewState.RaiseChanged();
            }
            catch
            {
                //TODO: A HANDLE ERROR
            }
            finally
            {

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
