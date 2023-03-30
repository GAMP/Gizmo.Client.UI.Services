using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserQuickLaunchViewStateService : ViewStateServiceBase<UserQuickLaunchViewState>
    {
        #region CONSTRUCTOR
        public UserQuickLaunchViewStateService(UserQuickLaunchViewState viewState,
            ILogger<UserQuickLaunchViewStateService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion

        #region FUNCTIONS

        public void SetSelectedTabIndex(int selectedTabIndex)
        {
            ViewState.SelectedTabIndex = selectedTabIndex;
            ViewState.RaiseChanged();
        }

        #endregion
    }
}
