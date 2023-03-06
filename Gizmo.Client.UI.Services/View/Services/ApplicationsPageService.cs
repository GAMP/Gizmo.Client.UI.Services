using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using static System.Net.Mime.MediaTypeNames;

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
        private bool _loaded = false;
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES


        #endregion

        #region FUNCTIONS

        public async Task LoadApplicationsAsync()
        {
            if (_loaded) //TODO: A RESET SOMEHOW
                return;

            Random random = new Random();

            var enterprises = await _gizmoClient.EnterprisesGetAsync(new ApplicationEnterprisesFilter());

            var applications = await _gizmoClient.ApplicationsGetAsync(new ApplicationsFilter());
            ViewState.Applications = applications.Data.Select(a => new ApplicationViewState()
            {
                Id = a.Id,
                ApplicationGroupId = a.ApplicationCategoryId,
                Title = a.Title,
                Description = a.Description,
                PublisherId = a.PublisherId,
                ReleaseDate = a.ReleaseDate,
                //TODO: A
                ImageId = a.Id,
                Ratings = random.Next(0, 100),
                Rate = ((decimal)random.Next(1, 50)) / 10,
                DateAdded = new DateTime(2021, 3, 12),
                ApplicationGroupName = "Shooter"
            }).ToList();

            var executables = await _gizmoClient.ExecutablesGetAsync(new ApplicationExecutablesFilter() { });

            var executablesList = executables.Data.Select(a => new ExecutableViewState()
             {
                 Id = a.Id,
                 Caption = a.Caption,
                 //TODO: A
                 PersonalFiles = new List<string>() { "Personal File 1", "Personal File 2", "Personal File 3" }
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

                application.Executables = executablesList.Take(application.Id).ToList();

                /*var executables = await _gizmoClient.GetApplicationExecutablesAsync(new ApplicationExecutablesFilter() { ApplicationId = application.Id });

                application.Executables = executables.Data.Select(a => new ExecutableViewState()
                {
                    Id = a.Id,
                    Caption = a.Caption,
                    //TODO: A
                    PersonalFiles = new List<string>() { "Personal File 1", "Personal File 2", "Personal File 3" }
                }).ToList();*/
            }

            _loaded = true;
        }

		public async Task<ApplicationViewState> GetApplicationAsync(int applicationId)
		{
			return ViewState.Applications.Where(a => a.Id == applicationId).FirstOrDefault();
		}

		public async Task<ExecutableViewState> GetExecutableAsync(int executableId)
        {
			foreach (var item in ViewState.Applications)
			{
				var executable = item.Executables?.Where(a => a.Id == executableId).FirstOrDefault();

				if (executable != null)
				{
					return executable;
				}
			}

            return null;
		}

        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            var applicationGroups = await _gizmoClient.ApplicationGroupsGetAsync(new ApplicationGroupsFilter());
            ViewState.ApplicationGroups = applicationGroups.Data.Select(a => new ApplicationGroupViewState()
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();
        }
    }
}
