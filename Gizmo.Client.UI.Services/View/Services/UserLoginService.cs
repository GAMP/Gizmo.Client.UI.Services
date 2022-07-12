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

        public async Task SubmitAsync()
        {
            //always validate state on submission
            ViewState.IsValid = EditContext.Validate();

            //model validation is pending, we cant proceed
            if (ViewState.IsValid != true)
                return;

            try
            {
                //initiate login
            }
            catch
            {

            }
            finally
            {

            }

            //simulate task
            await Task.Delay(1000);

            ViewState.SetDefaults();
            ResetValidationErrors();

            NavigationService.NavigateTo("/home");
        }  

        public Task LogutAsync()
        {
            NavigationService.NavigateTo("/");
            return Task.CompletedTask;
        }
    }
}
