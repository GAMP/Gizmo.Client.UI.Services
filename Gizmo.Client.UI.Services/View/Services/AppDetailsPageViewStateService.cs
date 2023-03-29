using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Web;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.ApplicationDetailsRoute)]
    public sealed class AppDetailsPageViewStateService : ViewStateServiceBase<AppDetailsPageViewState>
    {
        #region CONSTRUCTOR
        public AppDetailsPageViewStateService(AppDetailsPageViewState viewState,
            ILogger<AppDetailsPageViewStateService> logger,
            IServiceProvider serviceProvider,
            AppViewStateLookupService appLookupService,
            AppExeViewStateLookupService appExeLookupService) : base(viewState, logger, serviceProvider)
        {
            _appLookupService = appLookupService;
            _appExeLookupService = appExeLookupService;
        }
        #endregion

        #region FIELDS
        private readonly AppViewStateLookupService _appLookupService;
        private readonly AppExeViewStateLookupService _appExeLookupService;
        #endregion

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (Uri.TryCreate(NavigationService.GetUri(), UriKind.Absolute, out var uri))
            {
                string? applicationId = HttpUtility.ParseQueryString(uri.Query).Get("ApplicationId");
                if (!string.IsNullOrEmpty(applicationId))
                {
                    if (int.TryParse(applicationId, out int id))
                    {
                        var applicationViewState = await _appLookupService.GetStateAsync(id, false, cancellationToken);
                        ViewState.Application = applicationViewState;

                        var executables = await _appExeLookupService.GetStatesAsync(cancellationToken);
                        ViewState.Executables = executables.Where(a => a.ApplicationId == id).ToList();

                        DebounceViewStateChanged();
                    }
                }
            }
        }

        #endregion
    }
}
