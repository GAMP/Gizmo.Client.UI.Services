using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationService : ValidatingViewStateServiceBase<UserRegistrationViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationService(UserRegistrationViewState viewState,
            ILogger<UserRegistrationService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
        }
        #endregion

        #region FUNCTIONS

        public Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            NavigationService.NavigateTo("/registrationstep2");
            return Task.CompletedTask;
        }

        #endregion
    }
}
