using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserPasswordRecoveryService : ViewStateServiceBase<UserPasswordRecoveryViewState>
    {
        #region CONTRUCTOR
        public UserPasswordRecoveryService(UserPasswordRecoveryViewState viewState,
            UserPasswordRecoveryMethodViewState methodState,
            ILogger<UserPasswordRecoveryService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
            _passwordRecoveryMethodState = methodState;
        }
        #endregion

        #region FIELDS
        private readonly UserPasswordRecoveryMethodViewState _passwordRecoveryMethodState; 
        #endregion

        public Task StartAsync()
        {
            if(_passwordRecoveryMethodState.Method == UserPasswordRecoveryMethod.Email)
            {
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

    }
}
