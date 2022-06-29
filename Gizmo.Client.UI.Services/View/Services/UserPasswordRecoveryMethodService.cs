using Gizmo.Client.UI.View.States;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    public sealed class UserPasswordRecoveryMethodService : ViewStateServiceBase<UserPasswordRecoveryMethodViewState>
    {
        #region CONSTRUCTOR
        public UserPasswordRecoveryMethodService(UserPasswordRecoveryMethodViewState viewState,
           ILogger<UserPasswordRecoveryMethodService> logger) : base(viewState, logger)
        {

        }
        #endregion

        public Task SetMethodAsync(UserPasswordRecoveryMethod method)
        {
            ViewState.Method = method;
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }
    }
}
