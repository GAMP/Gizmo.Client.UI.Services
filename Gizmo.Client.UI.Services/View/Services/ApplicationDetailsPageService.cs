using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ApplicationDetailsPageService : ViewStateServiceBase<ApplicationDetailsPageViewState>
    {
        #region CONSTRUCTOR
        public ApplicationDetailsPageService(ApplicationDetailsPageViewState viewState,
            ILogger<ApplicationDetailsPageService> logger,
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

            //TODO: A Load application from cache or get by id?
            var applications = await _gizmoClient.GetApplicationsAsync(new ApplicationsFilter());
            ViewState.Application = applications.Data.Where(a => a.Id == id).Select(a => new ApplicationViewState()
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
            }).FirstOrDefault();

            //TODO: A LOAD ENTERPRISE

            var executables = await _gizmoClient.GetApplicationExecutablesAsync(new ApplicationExecutablesFilter() { ApplicationId = ViewState.Application.Id });

            //Test only.
            if (ViewState.Application.Id > 1)
            {
                ViewState.Application.Executables = executables.Data.Select(a => new ExecutableViewState()
                {
                    Id = a.Id,
                    Caption = a.Caption
                }).ToList();
            }
        }

        #endregion
    }
}
