using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserPasswordRecoverySetNewPasswordService : ValidatingViewStateServiceBase<UserPasswordRecoverySetNewPasswordViewState>
    {
        #region CONTRUCTOR
        public UserPasswordRecoverySetNewPasswordService(UserPasswordRecoverySetNewPasswordViewState viewState,
            ILogger<UserPasswordRecoveryService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region FUNCTIONS

        public void SetNewPassword(string value)
        {
            ViewState.NewPassword = value;
            ViewState.RaiseChanged();
        }

        public void SetRepeatPassword(string value)
        {
            ViewState.RepeatPassword = value;
            ViewState.RaiseChanged();
        }

        public async Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            var userPasswordRecoveryViewState = ServiceProvider.GetRequiredService<UserPasswordRecoveryViewState>();
            var userPasswordRecoveryConfirmationViewState = ServiceProvider.GetRequiredService<UserPasswordRecoveryConfirmationViewState>();

            try
            {
                var token = userPasswordRecoveryViewState.Token;
                var confirmationCode = userPasswordRecoveryConfirmationViewState.ConfirmationCode;
                var newPassword = ViewState.NewPassword;

                var result = await _gizmoClient.UserPasswordRecoveryCompleteAsync(token, confirmationCode, newPassword);

                if (result != PasswordRecoveryCompleteResultCode.Success)
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString("PASSWORD_RESET_FAILED_MESSAGE");

                    return;
                }

                //TODO: AAA SUCCESS MESSAGE?
                NavigationService.NavigateTo(ClientRoutes.LoginRoute);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Password recovery complete error.");

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

        protected override void OnCustomValidation(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            base.OnCustomValidation(fieldIdentifier, validationMessageStore);

            if (fieldIdentifier.FieldName == nameof(ViewState.NewPassword) || fieldIdentifier.FieldName == nameof(ViewState.RepeatPassword))
            {
                if (!string.IsNullOrEmpty(ViewState.NewPassword) && !string.IsNullOrEmpty(ViewState.RepeatPassword) && string.Compare(ViewState.NewPassword, ViewState.RepeatPassword) != 0)
                {
                    validationMessageStore.Add(() => ViewState.RepeatPassword, _localizationService.GetString("PASSWORDS_DO_NOT_MATCH"));
                }
            }
        }

        #endregion
    }
}
