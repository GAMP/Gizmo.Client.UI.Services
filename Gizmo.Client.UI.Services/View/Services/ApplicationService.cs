using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ApplicationService : ViewStateServiceBase<ApplicationViewState>
    {
        #region CONSTRUCTOR
        public ApplicationService(ApplicationViewState viewState,
            ILogger<ApplicationService> logger,
            IServiceProvider serviceProvider, IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;

            viewState.Executables = _gizmoClient.GetExecutables().Select(a => new ExecutableViewState()
            {
                Id = a.Id,
                Name = a.Caption
            }).ToList();
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES


        #endregion

        #region FUNCTIONS

        #endregion
    }
}
