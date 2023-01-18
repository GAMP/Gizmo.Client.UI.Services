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

        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            var executables = await _gizmoClient.GetApplicationExecutablesAsync(new ApplicationExecutablesFilter());
            ViewState.Executables = executables.Data.Select(a => new ExecutableViewState()
            {
                Id = a.Id,
                Caption = a.Caption,
                ImageId = null,
                //TODO: A
                PersonalFiles = new List<string>() { "Personal File 1", "Personal File 2", "Personal File 3" }
            }).ToList();

            ViewState.Executables[0].State = 0;
            ViewState.Executables[1].State = 1;
            ViewState.Executables[2].State = 2;
            ViewState.Executables[2].StatePercentage = 43.5m;
            ViewState.Executables[3].State = 3;

            for (int i = 0; i < 30; i++)
            {
                ViewState.Executables.Add(new ExecutableViewState()
                {
                    Id = 12,
                    Caption = "Test",
                    ImageId = null,
                    //TODO: A
                    PersonalFiles = new List<string>() { "Personal File 1", "Personal File 2", "Personal File 3" }
                });
            }
        }
    }
}