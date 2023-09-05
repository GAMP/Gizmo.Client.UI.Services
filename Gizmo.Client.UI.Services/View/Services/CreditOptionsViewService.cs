using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class CreditOptionsViewService : ViewStateServiceBase<CreditOptionsViewState>
    {
        public CreditOptionsViewService(CreditOptionsViewState viewState,
            ILogger<FeedsViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
    }
}
