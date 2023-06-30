using System.Globalization;

using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class ClientLocalizationViewService : ViewStateServiceBase<ClientLocalizationViewState>
    {
        #region CONSTRUCTOR
        public ClientLocalizationViewService(
            ClientLocalizationViewState viewState,
            NavigationService navigationService,
            IOptionsMonitor<ClientInterfaceOptions> clientInterfaceOptions,
            ILocalizationService localizationService,
            ILogger<ClientLocalizationViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _navigationService = navigationService;
            _clientInterfaceOptions = clientInterfaceOptions;
            _localizationService.LocalizationOptionsChanged += OnLocalizationOptionsChanged;
            _logger = logger;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly ILogger<ClientLocalizationViewService> _logger;
        private readonly IOptionsMonitor<ClientInterfaceOptions> _clientInterfaceOptions;
        private readonly NavigationService _navigationService;

        #endregion

        #region OVERRIDES

        protected override async Task OnInitializing(CancellationToken cToken)
        {
            ViewState.AvailableCultures = await _localizationService.GetSupportedCulturesAsync(cToken);

            var preferedLanguage = _clientInterfaceOptions.CurrentValue.PreferedLanguage;
            CultureInfo? preferedCulture = null;

            if (!string.IsNullOrWhiteSpace(preferedLanguage))
                preferedCulture = GetViewStatesCulture(preferedLanguage);

            preferedCulture ??= GetViewStatesCulture("en");

            ViewState.CurrentCulture = preferedCulture;

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

        #region PUBLIC FUNCTIONS

        public async void SetCurrentCultureAsync(string twoLetterISOLanguageName)
        {
            ViewState.CurrentCulture = GetViewStatesCulture(twoLetterISOLanguageName);

            await _localizationService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            ViewState.RaiseChanged();

            //TODO need to find a better way to do this
            var currentUri = _navigationService.GetUri();
            _navigationService.NavigateTo(currentUri ?? "/", new Microsoft.AspNetCore.Components.NavigationOptions() { ForceLoad = true });
        }

        #endregion

        #region PRIVATE FUNCTIONS

        private CultureInfo GetViewStatesCulture(string twoLetterISOLanguageName)
        {
            var culture = ViewState.AvailableCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName);

            if (culture == null)
            {
                _logger.LogWarning("Culture '{twoLetterName}' was not found. Using default culture 'en'.", twoLetterISOLanguageName);

                culture = ViewState.AvailableCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == "en");

                if (culture == null)
                    throw new CultureNotFoundException($"Culture {twoLetterISOLanguageName} not found.");
            }

            return culture;
        }

        private async void OnLocalizationOptionsChanged(object? _, EventArgs __)
        {
            ViewState.AvailableCultures = await _localizationService.GetSupportedCulturesAsync(default);
            ViewState.CurrentCulture = GetViewStatesCulture(ViewState.CurrentCulture.TwoLetterISOLanguageName);
            await _localizationService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            ViewState.RaiseChanged();
        }

        #endregion
    }
}
