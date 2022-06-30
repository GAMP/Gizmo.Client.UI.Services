using Gizmo.Client.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserPasswordRecoveryMethodService : ClientViewServiceBase<UserPasswordRecoveryMethodViewState>
    {
        #region CONSTRUCTOR
        public UserPasswordRecoveryMethodService(UserPasswordRecoveryMethodViewState viewState,
           ILogger<UserPasswordRecoveryMethodService> logger,
           IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
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
