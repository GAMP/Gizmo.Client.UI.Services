using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserPasswordRecoveryService : ValidatingViewStateServiceBase<UserPasswordRecoveryViewState>
    {
        #region CONTRUCTOR
        public UserPasswordRecoveryService(UserPasswordRecoveryViewState viewState,
            ILogger<UserPasswordRecoveryService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion

        #region FIELDS
        #endregion

        #region FUNCTIONS

        public void SetRecoveryMethod(UserPasswordRecoveryMethod userPasswordRecoveryMethod)
        {
            ViewState.Method = userPasswordRecoveryMethod;

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

            }
            finally
            {

            }
        }

        #endregion

        #region OVERRIDES

        protected override void OnCustomValidation(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            base.OnCustomValidation(fieldIdentifier, validationMessageStore);

            if (ViewState.Method == UserPasswordRecoveryMethod.Email &&
                fieldIdentifier.FieldName == nameof(ViewState.Email) &&
                string.IsNullOrEmpty(ViewState.Email))
            {
                validationMessageStore.Add(() => ViewState.Email, "The e-mail field is required.");
            }


            if (ViewState.Method == UserPasswordRecoveryMethod.MobilePhone &&
                fieldIdentifier.FieldName == nameof(ViewState.MobilePhone) &&
                string.IsNullOrEmpty(ViewState.MobilePhone))
            {
                validationMessageStore.Add(() => ViewState.MobilePhone, "The phone number field is required.");
            }
        }

        #endregion
    }
}