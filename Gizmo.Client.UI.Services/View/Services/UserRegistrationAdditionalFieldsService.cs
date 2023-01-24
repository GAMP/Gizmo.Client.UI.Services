using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationAdditionalFieldsService : ValidatingViewStateServiceBase<UserRegistrationAdditionalFieldsViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationAdditionalFieldsService(UserRegistrationAdditionalFieldsViewState viewState,
            ILogger<UserRegistrationAdditionalFieldsService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
        }
        #endregion

        #region FUNCTIONS

        public void SetHomePhone(string value)
        {
            ViewState.HomePhone = value;
            ViewState.RaiseChanged();
        }

        public Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            NavigationService.NavigateTo(ClientRoutes.LoginRoute);
            return Task.CompletedTask;
        }

        #endregion
    }
}
