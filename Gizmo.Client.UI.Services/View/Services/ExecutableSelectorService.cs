using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ExecutableSelectorService : ViewStateServiceBase<ExecutableSelectorViewState>
    {
        #region CONSTRUCTOR
        public ExecutableSelectorService(ExecutableSelectorViewState viewState,
            ILogger<ExecutableSelectorService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion
    }
}
