using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
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
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            ViewState.ConfirmationMethod = UserRegistrationMethod.Email;
        }
        #endregion

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

                ViewState.CanResend = false;

                NavigationService.NavigateTo("/registrationconfirmation");

                //TODO: A
                ViewState.CanResend = false;
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
    }
}
