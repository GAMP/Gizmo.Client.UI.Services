using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class HostLockViewStateService : ViewStateServiceBase<HostLockViewState>
    {
        public HostLockViewStateService(HostLockViewState viewState,ILogger<HostLockViewState> logger, IServiceProvider serviceProvider)
            :base(viewState, logger, serviceProvider)
        {
        }
    }
}
