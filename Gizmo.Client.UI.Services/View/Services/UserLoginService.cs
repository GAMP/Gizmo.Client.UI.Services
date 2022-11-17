using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserLoginService : ValidatingViewStateServiceBase<UserLoginViewState>, IDisposable
    {
        #region CONSTRUCTOR
        public UserLoginService(UserLoginViewState viewState,
            ILogger<UserLoginService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion

        #region FUNCTIONS

        public void SetLoginMethod(UserLoginType userLoginType)
        {
            ViewState.LoginType = userLoginType;

            ViewState.RaiseChanged();
        }

        public async Task LoginAsync()
        {
            //always validate state on submission
            ViewState.IsValid = EditContext.Validate();

            //model validation is pending, we cant proceed
            if (ViewState.IsValid != true)
                return;

            ViewState.IsLogginIn = true;
            ViewState.RaiseChanged();

            try
            {
                //simulate login task
                await Task.Delay(1000);

                //initiate login
                if (ViewState.LoginName == "1")
                {
                    //Failed login
                    ViewState.IsLogginIn = false;
                    ViewState.HasLoginErrors = true;
                    ViewState.RaiseChanged();

                    //Reset after some time?
                    await Task.Delay(10000);

                    ViewState.HasLoginErrors = false;
                    ViewState.SetDefaults();
                    ResetValidationErrors();

                    ViewState.RaiseChanged();
                }
                else
                {
                    //Successful login
                    NavigationService.NavigateTo("/home");

                    ViewState.IsLogginIn = false;
                    ViewState.HasLoginErrors = false;
                    ViewState.SetDefaults();

                    ResetValidationErrors();

                    ViewState.RaiseChanged();
                }
            }
            catch
            {

            }
            finally
            {

            }
        }

        public Task OpenRegistrationAsync()
        {
            var userRegistrationConfirmationMethodService = ServiceProvider.GetRequiredService<UserRegistrationConfirmationMethodService>();
            return userRegistrationConfirmationMethodService.StartRegistrationAsync();
        }

        public Task LogοutAsync()
        {
            NavigationService.NavigateTo("/");
            return Task.CompletedTask;
        }

        #endregion
    }
}