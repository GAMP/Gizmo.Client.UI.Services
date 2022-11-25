using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserPasswordRecoveryConfirmationService : ValidatingViewStateServiceBase<UserPasswordRecoveryConfirmationViewState>
    {
        #region CONTRUCTOR
        public UserPasswordRecoveryConfirmationService(UserPasswordRecoveryConfirmationViewState viewState,
            UserPasswordRecoveryMethodViewState methodState,
            ILogger<UserPasswordRecoveryService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _passwordRecoveryMethodState = methodState;
        }
        #endregion

        #region FIELDS
        private readonly UserPasswordRecoveryMethodViewState _passwordRecoveryMethodState;
        #endregion

        public Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            NavigationService.NavigateTo(ClientRoutes.PasswordRecoverySetNewPasswordRoute);
            return Task.CompletedTask;
        }
    }
}
