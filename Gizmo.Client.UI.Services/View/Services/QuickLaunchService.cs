using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class QuickLaunchService : ViewStateServiceBase<QuickLaunchViewState>
    {
        #region CONSTRUCTOR
        public QuickLaunchService(QuickLaunchViewState viewState,
            ILogger<QuickLaunchService> logger,
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

            ViewState.Executables.Add(new ExecutableViewState() { Id = 1, Caption = "Explorer", Image = "_content/Gizmo.Client.UI/img/Places-folder-red-icon_1.png" });
            ViewState.Executables.Add(new ExecutableViewState() { Id = 2, Caption = "Word", Image = "_content/Gizmo.Client.UI/img/Word-2-icon_1.png" });
            ViewState.Executables.Add(new ExecutableViewState() { Id = 3, Caption = "DOTA", Image = "_content/Gizmo.Client.UI/img/dota-2-icon_1.png" });
            ViewState.Executables.Add(new ExecutableViewState() { Id = 4, Caption = "Spotify", Image = "_content/Gizmo.Client.UI/img/spotify-512.png" });
            ViewState.Executables.Add(new ExecutableViewState() { Id = 5, Caption = "BattleNet", Image = "_content/Gizmo.Client.UI/img/bNet.png" });
            ViewState.Executables.Add(new ExecutableViewState() { Id = 6, Caption = "Chrome", Image = "_content/Gizmo.Client.UI/img/Chrome-icon_1.png" });
        }
    }
}
