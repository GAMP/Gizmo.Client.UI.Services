using Gizmo.Client.UI.View.States;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    public sealed class UserRegistrationMethodService : ViewStateServiceBase<UserRegistrationMethodViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationMethodService(UserRegistrationMethodViewState viewState,
            ILogger<UserRegistrationMethodService> logger) : base(viewState, logger)
        {
        }   
        #endregion

        public Task SetMethodAsync(UserRegistrationMethod method)
        {
            return Task.CompletedTask;
        }

    }
}
