using Gizmo.Client.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationMethodService : ClientViewServiceBase<UserRegistrationMethodViewState>
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
            return Task.CompletedTask;
        }

    }
}
