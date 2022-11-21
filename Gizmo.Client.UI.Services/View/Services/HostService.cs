using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class HostService : ViewStateServiceBase<HostViewState>
    {
        #region CONSTRUCTOR
        public HostService(HostViewState viewState,
            ILogger<HostService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        public HostConfigurationViewState HostConfigurationViewState { get; set; } = new HostConfigurationViewState();

        #endregion

        #region FUNCTIONS

        public Task SetHostLockStateAsync(bool value)
        {
            //Test only.
            ViewState.IsLocked = value;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion
    }
}
