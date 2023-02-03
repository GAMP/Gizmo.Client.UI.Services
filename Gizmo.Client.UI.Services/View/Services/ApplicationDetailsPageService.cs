using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
	[Route(ClientRoutes.ApplicationDetailsRoute)]
	public sealed class ApplicationDetailsPageService : ViewStateServiceBase<ApplicationDetailsPageViewState>
    {
        #region CONSTRUCTOR
        public ApplicationDetailsPageService(ApplicationDetailsPageViewState viewState,
            ILogger<ApplicationDetailsPageService> logger,
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

        private async Task LoadApplicationAsync(int id)
        {
            //TODO: A Load application from cache or get by id?
            var applicationsPageService = ServiceProvider.GetRequiredService<ApplicationsPageService>();

            if (applicationsPageService != null)
            {
                ViewState.Application = await applicationsPageService.GetApplicationAsync(id);
            }

            //TODO: A Load enterprise to update publisher name in application view state.
        }

        #endregion

        protected override async Task OnNavigatedIn()
        {
            await base.OnNavigatedIn();

            if (Uri.TryCreate(NavigationService.GetUri(), UriKind.Absolute, out var uri))
            {
                string? applicationId = HttpUtility.ParseQueryString(uri.Query).Get("ApplicationId");
                if (!string.IsNullOrEmpty(applicationId))
                {
                    if (int.TryParse(applicationId, out int id))
                    {
                        await LoadApplicationAsync(id);
                    }
                }
            }
        }
    }
}
