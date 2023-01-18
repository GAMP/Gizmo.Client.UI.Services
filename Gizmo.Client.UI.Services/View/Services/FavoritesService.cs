using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class FavoritesService : ViewStateServiceBase<FavoritesViewState>
    {
        #region CONSTRUCTOR
        public FavoritesService(FavoritesViewState viewState,
            ILogger<FavoritesService> logger,
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

            ViewState.Executables.Add(new ExecutableViewState() { Id = 1, Caption = "Explorer", ImageId = null });
            ViewState.Executables.Add(new ExecutableViewState() { Id = 2, Caption = "Word", ImageId = null, State = ExecutableState.Loading });
            ViewState.Executables.Add(new ExecutableViewState() { Id = 3, Caption = "DOTA", ImageId = null, State = ExecutableState.Deployment, StatePercentage = 80 });
            ViewState.Executables.Add(new ExecutableViewState() { Id = 4, Caption = "Spotify", ImageId = null, State = ExecutableState.Running });
            ViewState.Executables.Add(new ExecutableViewState() { Id = 5, Caption = "BattleNet", ImageId = null });
        }
    }
}
