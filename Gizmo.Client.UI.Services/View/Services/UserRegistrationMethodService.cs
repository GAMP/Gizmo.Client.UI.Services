using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationMethodService : ViewStateServiceBase<UserRegistrationMethodViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationMethodService(UserRegistrationMethodViewState viewState,
            ILogger<UserRegistrationMethodService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
        }   
        #endregion

        public Task SetMethodAsync(UserRegistrationMethod method)
        {
            ViewState.Method = UserPasswordRecoveryMethod.MobilePhone;
            return Task.CompletedTask;
        }

    }
}
