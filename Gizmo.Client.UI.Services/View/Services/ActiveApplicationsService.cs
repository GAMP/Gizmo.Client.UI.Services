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
            var favoritesService = ServiceProvider.GetRequiredService<FavoritesService>();

            await favoritesService.AddToRecentAsync(executableId);

            //Check if executable already exists in the list.
            var executable = ViewState.Executables.Where(a => a.Id == executableId).FirstOrDefault();

            if (executable == null)
            {
                var applicationsPageService = ServiceProvider.GetRequiredService<ApplicationsPageService>();

                if (applicationsPageService == null)
                    return;

                executable = await applicationsPageService.GetExecutableAsync(executableId);

                if (executable == null)
                    return;

                //Add executable view state to the list.
                ViewState.Executables.Add(executable);
                ViewState.RaiseChanged();
            }

            //TODO: A 

            //Change executable state.
            executable.State = ExecutableState.Deployment;
            executable.RaiseChanged();

            //TODO: A 

            await Task.Delay(5000);

            //Change executable state.
            executable.State = ExecutableState.Running;
            executable.RaiseChanged();
        }

        public async Task TerminateExecutableAsyc(int executableId)
        {
            //Check if executable already exists in the list.
            var executable = ViewState.Executables.Where(a => a.Id == executableId).FirstOrDefault();

            if (executable != null)
            {
                //Change executable state.
                executable.State = ExecutableState.Terminating;
                executable.RaiseChanged();

                await Task.Delay(1000);

                ViewState.Executables.Remove(executable);
                ViewState.RaiseChanged();
            }
        }

        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

        }
    }
}