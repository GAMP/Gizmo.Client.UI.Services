using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserQuickLaunchService : ViewStateServiceBase<UserQuickLaunchViewState>
    {
        #region CONSTRUCTOR
        public UserQuickLaunchService(UserQuickLaunchViewState viewState,
            ILogger<UserQuickLaunchService> logger,
            IServiceProvider serviceProvider, IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);


        }

        public void SetSelectedTabIndex(int selectedTabIndex)
        {
            ViewState.SelectedTabIndex = selectedTabIndex;
            ViewState.RaiseChanged();
        }
    }
}
