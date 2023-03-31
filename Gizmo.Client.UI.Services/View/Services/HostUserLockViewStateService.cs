using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class HostUserLockViewStateService : ViewStateServiceBase<HostUserLockViewState>
    {
        public HostUserLockViewStateService(HostUserLockViewState viewState,ILogger<HostUserLockViewState> logger, IServiceProvider serviceProvider)
            :base(viewState, logger, serviceProvider)
        {
        }

        public Task SetHostLockStateAsync(bool value)
        {
            ViewState.IsLocked = value;
            DebounceViewStateChanged();
            return Task.CompletedTask;
        }
    }
}
