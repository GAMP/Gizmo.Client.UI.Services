using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class RegistrationConfirmationMethodViewService : ViewStateServiceBase<RegistrationConfirmationMethodViewState>
    {
        #region CONSTRUCTOR
        public RegistrationConfirmationMethodViewService(RegistrationConfirmationMethodViewState viewState,
            ILogger<RegistrationConfirmationMethodViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
        }
        #endregion

        #region FUNCTIONS

        public Task SetMethodAsync(RegistrationVerificationMethod method)
        {
            ViewState.ConfirmationMethod = method;
            return Task.CompletedTask;
        }

        #endregion
    }
}
