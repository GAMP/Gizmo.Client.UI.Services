using Gizmo.Client.UI.View.States;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    public sealed class UserPasswordRecoveryService : ViewStateServiceBase<UserPasswordRecoveryViewState>
    {
        #region CONTRUCTOR
        public UserPasswordRecoveryService(UserPasswordRecoveryViewState viewState,
            UserPasswordRecoveryMethodViewState methodState,
            ILogger<UserPasswordRecoveryService> logger) : base(viewState, logger)
        {
            _passwordRecoveryMethodState = methodState;
        }
        #endregion

        #region FIELDS
        private readonly UserPasswordRecoveryMethodViewState _passwordRecoveryMethodState; 
        #endregion

        public Task StartAsync()
        {
            return Task.CompletedTask;
        }

    }
}
