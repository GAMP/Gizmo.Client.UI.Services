using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Web;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.ApplicationDetailsRoute)]
    public sealed class AppDetailsPageViewService : ViewStateServiceBase<AppDetailsPageViewState>
    {
        #region CONSTRUCTOR
        public AppDetailsPageViewService(AppDetailsPageViewState viewState,
            ILogger<AppDetailsPageViewService> logger,
            IServiceProvider serviceProvider,
            AppViewStateLookupService appLookupService,
            AppExeViewStateLookupService appExeLookupService,
            IOptions<ClientInterfaceOptions> clientUIOptions) : base(viewState, logger, serviceProvider)
        {
            _appLookupService = appLookupService;
            _appExeLookupService = appExeLookupService;
            _clientUIOptions = clientUIOptions;
        }
        #endregion

        #region FIELDS
        private readonly AppViewStateLookupService _appLookupService;
        private readonly AppExeViewStateLookupService _appExeLookupService;
        private readonly IOptions<ClientInterfaceOptions> _clientUIOptions;
        #endregion

        #region OVERRIDES

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.DisableAppDetails = _clientUIOptions.Value.DisableAppDetails;
            return base.OnInitializing(ct);
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (Uri.TryCreate(NavigationService.GetUri(), UriKind.Absolute, out var uri))
            {
                string? applicationId = HttpUtility.ParseQueryString(uri.Query).Get("ApplicationId");
                if (!string.IsNullOrEmpty(applicationId))
                {
                    if (int.TryParse(applicationId, out int id))
                    {
                        ViewState.Application = await _appLookupService.GetStateAsync(id, false, cancellationToken);
                        ViewState.Executables = await _appExeLookupService.GetFilteredStatesAsync(id, cancellationToken);
                        DebounceViewStateChanged();
                    }
                }
            }
        }

        #endregion
    }
}
