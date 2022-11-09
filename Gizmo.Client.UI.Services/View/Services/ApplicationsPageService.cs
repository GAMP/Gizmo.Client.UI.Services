using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ApplicationsPageService : ViewStateServiceBase<ApplicationsPageViewState>
    {
        #region CONSTRUCTOR
        public ApplicationsPageService(ApplicationsPageViewState viewState,
            ILogger<ApplicationsPageService> logger,
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

            Random random = new Random();

            var applicationGroups = await _gizmoClient.GetApplicationGroupsAsync(new ApplicationGroupsFilter());
            ViewState.ApplicationGroups = applicationGroups.Data.Select(a => new ApplicationGroupViewState()
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();

            var applications = await _gizmoClient.GetApplicationsAsync(new ApplicationsFilter());
            ViewState.Applications = applications.Data.Select(a => new ApplicationViewState()
            {
                Id = a.Id,
                ApplicationGroupId = a.ApplicationCategoryId,
                Title = a.Title,
                ImageId = null,
                Ratings = random.Next(0, 100),
                Rate = ((decimal)random.Next(1, 50)) / 10,
                ReleaseDate = new DateTime(2019, 10, 22),
                DateAdded = new DateTime(2021, 3, 12)
            }).ToList();

            var executables = await _gizmoClient.GetApplicationExecutablesAsync(new ApplicationExecutablesFilter());
            foreach (var item in ViewState.Applications)
            {
                if (item.Id > 1)
                {
                    item.Executables = executables.Data.Select(a => new ExecutableViewState()
                    {
                        Id = a.Id,
                        Caption = a.Caption
                    }).ToList();
                }
            }
        }
    }
}
