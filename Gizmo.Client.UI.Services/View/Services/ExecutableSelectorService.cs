using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ExecutableSelectorService : ViewStateServiceBase<ExecutableSelectorViewState>
    {
        #region CONSTRUCTOR
        public ExecutableSelectorService(ExecutableSelectorViewState viewState,
            ILogger<ExecutableSelectorService> logger,
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

        public async Task LoadApplicationAsync(int id)
        {
            Random random = new Random();

            var applications = await _gizmoClient.GetApplicationsAsync(new ApplicationsFilter());
            ViewState.Application = applications.Data.Where(a => a.Id == id).Select(a => new ApplicationViewState()
            {
                Id = a.Id,
                ApplicationGroupId = a.ApplicationCategoryId,
                Title = a.Title,
                ImageId = null,
                Ratings = random.Next(0, 100),
                Rate = ((decimal)random.Next(1, 50)) / 10,
                ReleaseDate = new DateTime(2019, 10, 22),
                DateAdded = new DateTime(2021, 3, 12),
                ApplicationGroupName = "Shooter"
            }).FirstOrDefault();

            var executables = await _gizmoClient.GetApplicationExecutablesAsync(new ApplicationExecutablesFilter());
            ViewState.Application.Executables = executables.Data.Select(a=> new ExecutableViewState()
            {
                Id = a.Id,
                Caption = a.Caption,
                ImageId = null,
                //TODO: A
                PersonalFiles = new List<string>() { "Personal File 1", "Personal File 2", "Personal File 3" }
            }).ToList();

            ViewState.Application.Executables[0].State = ExecutableState.None;
            ViewState.Application.Executables[1].State = ExecutableState.Loading;
            ViewState.Application.Executables[2].State = ExecutableState.Deployment;
            ViewState.Application.Executables[2].StatePercentage = 43.5m;
            ViewState.Application.Executables[3].State = ExecutableState.Running;
        }

        #endregion
    }
}