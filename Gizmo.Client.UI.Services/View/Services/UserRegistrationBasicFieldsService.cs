using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationBasicFieldsService : ValidatingViewStateServiceBase<UserRegistrationBasicFieldsViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationBasicFieldsService(UserRegistrationBasicFieldsViewState viewState,
            ILogger<UserRegistrationBasicFieldsService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion

        #region FUNCTIONS

        public void SetUsername(string value)
        {
            ViewState.Username = value;
            ViewState.RaiseChanged();
        }

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

        public Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            NavigationService.NavigateTo(ClientRoutes.RegistrationAdditionalFieldsRoute);
            return Task.CompletedTask;
        }

        #endregion
    }
}