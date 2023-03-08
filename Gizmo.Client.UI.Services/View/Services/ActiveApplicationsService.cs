using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ActiveApplicationsService : ViewStateServiceBase<ActiveApplicationsViewState>
    {
        #region CONSTRUCTOR
        public ActiveApplicationsService(ActiveApplicationsViewState viewState,
            ILogger<ActiveApplicationsService> logger,
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

        public async Task RunExecutableAsyc(int executableId)
        {
            //Add to favorites.
            var favoritesService = ServiceProvider.GetRequiredService<FavoritesService>();
            if (favoritesService != null)
            {
                await favoritesService.AddToRecentAsync(executableId);
            }

            //Check if executable already exists in the list.
            var executable = ViewState.Executables.Where(a => a.ExecutableId == executableId).FirstOrDefault();

            if (executable == null)
            {
                var applicationsPageService = ServiceProvider.GetRequiredService<ApplicationsPageService>();

                if (applicationsPageService == null)
                    return;

                //executable = await applicationsPageService.GetExecutableAsync(executableId);

                if (executable == null)
                    return;

                //Add executable view state to the list.
                var tmp = ViewState.Executables.ToList();
                tmp.Add(executable);
                ViewState.Executables = tmp;
                ViewState.RaiseChanged();
            }

            //TODO: A Run executable.
            
            //Test
            //Change executable state.
            //executable.State = ExecutableState.Deployment;
            //executable.RaiseChanged();

            //await Task.Delay(2000);

            //executable.State = ExecutableState.Loading;
            //executable.RaiseChanged();

            //await Task.Delay(2000);

            ////Change executable state.
            //executable.State = ExecutableState.Running;
            //End Test

            executable.RaiseChanged();
        }

        public async Task TerminateExecutableAsyc(int executableId)
        {
            //Check if executable already exists in the list.
            var executable = ViewState.Executables.Where(a => a.ExecutableId == executableId).FirstOrDefault();

            if (executable != null)
            {
                //TODO: A Terminate executable.

                //Test
                //Change executable state.
                //executable.State = ExecutableState.Terminating;
                //executable.RaiseChanged();

                //await Task.Delay(1000);

                //executable.State = ExecutableState.None;
                //executable.RaiseChanged();

                //ViewState.Executables = ViewState.Executables.Where(a => a != executable).ToList();
                //End Test
                
                ViewState.RaiseChanged();
            }
        }

        #endregion
    }
}
