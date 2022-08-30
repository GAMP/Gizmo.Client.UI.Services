using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserLockService : ViewStateServiceBase<UserLockViewState>, IDisposable
    {
        #region CONSTRUCTOR
        public UserLockService(UserLockViewState viewState,
            ILogger<UserLockService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion

        public Task SetPasswordAsync()
        {
            ViewState.IsLocking = false;
            ViewState.IsLocked = true;
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }

        public Task CancelLockAsync()
        {
            ViewState.IsLocking = false;
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }

        public Task LockAsync()
        {
            ViewState.IsLocking = true;
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }

        public Task UnlockAsync()
        {
            ViewState.IsLocked = false;
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }
    }
}
