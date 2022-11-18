using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ApplicationsPageService : ViewStateServiceBase<ApplicationsPageViewState>
    {
        #region CONSTRUCTOR
        public ApplicationsPageService(ApplicationsPageViewState viewState,
            ILogger<ApplicationsPageService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
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

        public async Task LoadApplicationsAsync()
        {
            Random random = new Random();

            var enterprises = await _gizmoClient.GetAppEnterprisesAsync(new ApplicationEnterprisesFilter());

            var applications = await _gizmoClient.GetApplicationsAsync(new ApplicationsFilter());
            ViewState.Applications = applications.Data.Select(a => new ApplicationViewState()
            {
                Id = a.Id,
                ApplicationCategoryId = a.ApplicationCategoryId,
                Title = a.Title,
                Description = a.Description,
                PublisherId = a.PublisherId,
                ReleaseDate = a.ReleaseDate,
                //TODO: A
                ImageId = null,
                Ratings = random.Next(0, 100),
                Rate = ((decimal)random.Next(1, 50)) / 10,
                DateAdded = new DateTime(2021, 3, 12)
            }).ToList();

            foreach (var application in ViewState.Applications)
            {
                if (application.PublisherId.HasValue)
                {
                    var enterprise = enterprises.Data.Where(a => a.Id == application.PublisherId).FirstOrDefault();

                    if (enterprise != null)
                    {
                        application.Publisher = enterprise.Name;
                    }
                }

                var executables = await _gizmoClient.GetApplicationExecutablesAsync(new ApplicationExecutablesFilter() { ApplicationId = application.Id });

                //Test only.
                if (application.Id > 1)
                {
                    application.Executables = executables.Data.Select(a => new ExecutableViewState()
                    {
                        Id = a.Id,
                        Caption = a.Caption
                    }).ToList();
                }
            }
        }

        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            var applicationGroups = await _gizmoClient.GetApplicationGroupsAsync(new ApplicationGroupsFilter());
            ViewState.ApplicationGroups = applicationGroups.Data.Select(a => new ApplicationGroupViewState()
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();
        }
    }
}
