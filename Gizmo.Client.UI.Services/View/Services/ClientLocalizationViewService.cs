using System.Globalization;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class ClientLocalizationViewService : ViewStateServiceBase<ClientLocalizationViewState>
    {
        #region CONSTRUCTOR
        public ClientLocalizationViewService(
            ClientLocalizationViewState viewState,
            ILocalizationService localizationService,
            IServerInfoService serverInfo,
            ILogger<ClientLocalizationViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _serverInfo = serverInfo;
            _localizationService.LocalizationOptionsChanged += OnLocalizationOptionsChanged;
            _logger = logger;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly ILogger<ClientLocalizationViewService> _logger;
        private readonly IServerInfoService _serverInfo;
        private static readonly CultureInfo _fallbackCulture = CultureInfo.GetCultureInfo("en-US");

        #endregion

        #region OVERRIDES

        protected override async Task OnInitializing(CancellationToken cToken)
        {
            ViewState.CurrentCulture = await ResolveServerCultureAsync(cToken);
            await _localizationService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            await base.OnInitializing(cToken);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            if (isDisposing)
            {
                _localizationService.LocalizationOptionsChanged -= OnLocalizationOptionsChanged;
            }

            base.OnDisposing(isDisposing);
        }

        #endregion

        #region PRIVATE FUNCTIONS

        private async void OnLocalizationOptionsChanged(object? _, EventArgs __)
        {
            // Re-apply the current culture so localization options (e.g. currency) are reconfigured.
            if (ViewState.CurrentCulture is not null)
                await _localizationService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            ViewState.RaiseChanged();
        }

        private async Task<CultureInfo> ResolveServerCultureAsync(CancellationToken cancellationToken)
        {
            string? serverCulture;
            try
            {
                serverCulture = await _serverInfo.GetDefaultCultureAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load default culture from server.");
                serverCulture = null;
            }

            if (string.IsNullOrWhiteSpace(serverCulture))
                return _fallbackCulture;

            try
            {
                var culture = CultureInfo.GetCultureInfo(serverCulture);
                return culture.IsNeutralCulture ? CultureInfo.CreateSpecificCulture(culture.Name) : culture;
            }
            catch (CultureNotFoundException ex)
            {
                _logger.LogWarning(ex, "Server default culture '{culture}' is not valid. Falling back to '{fallback}'.", serverCulture, _fallbackCulture.Name);
                return _fallbackCulture;
            }
        }

        #endregion
    }
}
