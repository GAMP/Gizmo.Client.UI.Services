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

        public async Task LoadQuickLaunchAsync()
        {
            //TODO: A Load quick launch applications on user login?

            //Test
            var applicationsPageService = ServiceProvider.GetRequiredService<ApplicationsPageService>();
            var activeApplicationsService = ServiceProvider.GetRequiredService<ActiveApplicationsService>();

            if (applicationsPageService == null)
                return;

            await applicationsPageService.LoadApplicationsAsync();

            Random random = new Random();

            foreach (var application in applicationsPageService.ViewState.Applications)
            {
                foreach (var exe in application.Executables)
                {
                    if (!ViewState.Executables.Contains(exe))
                    {
                        ViewState.Executables.Add(exe);

                        exe.State = (ExecutableState)random.Next(0, 4);
                        exe.RaiseChanged();

                        if (exe.State != ExecutableState.None)
                        {
                            activeApplicationsService.ViewState.Executables.Add(exe);
                        }
                    }
                }
            }

            activeApplicationsService.ViewState.RaiseChanged();
            //End Test

            ViewState.RaiseChanged();
        }

        #endregion
    }
}