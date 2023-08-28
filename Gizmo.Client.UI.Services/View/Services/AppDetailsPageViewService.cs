using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
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
        public override bool ValidateCommand<TCommand>(TCommand command)
        {
            if (_clientUIOptions.Value.DisableAppDetails)
                return false;

            if (command.Type != ViewServiceCommandType.Navigate)
                return false;

            if (command.Params?.Any() != true)
                return false;

            var paramAppId = command.Params.GetValueOrDefault("appId")?.ToString();

            if (paramAppId is null)
                return false;

            return true;
        }

        public override async Task ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cToken = default)
        {
            if (_clientUIOptions.Value.DisableAppDetails)
                return;

            if (command.Params?.Any() != true)
                return;

            var paramAppId = command.Params.GetValueOrDefault("appId")?.ToString();

            if (paramAppId is null)
                return;

            var appId = int.Parse(paramAppId, NumberStyles.Number);

            var apps = await _appLookupService.GetStatesAsync();
            if (!apps.Where(a => a.ApplicationId == appId).Any())
            {
                NavigationService.NavigateTo(ClientRoutes.NotFoundRoute);
                return;
            }

            switch (command.Type)
            {
                case ViewServiceCommandType.Navigate:
                    NavigationService.NavigateTo(ClientRoutes.ApplicationDetailsRoute + "?ApplicationId=" + paramAppId);
                    break;
            }
        }

        #endregion
    }
}
