using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserPasswordRecoveryMethodService : ViewStateServiceBase<UserPasswordRecoveryMethodViewState>
    {
        #region CONSTRUCTOR
        public UserPasswordRecoveryMethodService(UserPasswordRecoveryMethodViewState viewState,
           ILogger<UserPasswordRecoveryMethodService> logger,
           IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {

        }
        #endregion
    }
}
