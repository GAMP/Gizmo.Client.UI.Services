using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserChangeMobileService : ValidatingViewStateServiceBase<UserChangeMobileViewState>
    {
        #region CONSTRUCTOR
        public UserChangeMobileService(UserChangeMobileViewState viewState,
            ILogger<UserChangeMobileService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion

        public Task SendConfirmationCodeAsync()
        {
            ViewState.PageIndex = 1;
            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public Task VerifyAsync()
        {
            ViewState.PageIndex = 2;
            ViewState.IsComplete = true;
            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public Task ResendAsync()
        {
            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public Task ResetAsync()
        {
            ViewState.PageIndex = 0;
            ViewState.IsComplete = false;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }
    }
}