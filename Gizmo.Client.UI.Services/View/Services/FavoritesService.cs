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

        public async Task AddToRecentAsync(int executableId)
        {
            if (!ViewState.Executables.Where(a => a.ExecutableId == executableId).Any())
            {
                var applicationsPageService = ServiceProvider.GetRequiredService<ApplicationsPageService>();

                if (applicationsPageService == null)
                    return;

                AppExeViewState executable = null; // = await applicationsPageService.GetExecutableAsync(executableId);

                if (executable == null)
                    return;

                var tmp = ViewState.Executables.ToList();
                tmp.Add(executable);
                ViewState.Executables = tmp;
                ViewState.RaiseChanged();
            }
        }

        #endregion
    }
}
