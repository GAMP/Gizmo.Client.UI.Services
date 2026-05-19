using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.RegistrationProvidersRoute)]
    public sealed class RegistrationProvidersViewService : ViewStateServiceBase<RegistrationProvidersViewState>
    {
        #region CONSTRUCTOR
        public RegistrationProvidersViewService(
            RegistrationProvidersViewState viewState,
            ILogger<RegistrationProvidersViewService> logger,
            IServiceProvider serviceProvider,
            IUserRegistrationService registrationService,
            UserRegistrationViewService userRegistrationViewService,
            UserRegistrationViewState userRegistrationViewState,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _registrationService = registrationService;
            _userRegistrationViewService = userRegistrationViewService;
            _userRegistrationViewState = userRegistrationViewState;
            _localizationService = localizationService;
        }
        #endregion

        #region FIELDS
        private readonly IUserRegistrationService _registrationService;
        private readonly UserRegistrationViewService _userRegistrationViewService;
        private readonly UserRegistrationViewState _userRegistrationViewState;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region FUNCTIONS

        public Task SelectProviderAsync(Guid publicId)
        {
            var provider = ViewState.Providers.FirstOrDefault(p => p.PublicId == publicId);
            if (provider is null)
                return Task.CompletedTask;

            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.FailedChannelGuid = null;
            ViewState.RaiseChanged();

            if (provider.CanRedirect)
            {
                _userRegistrationViewService.SelectProvider(provider);
                NavigationService.NavigateTo(ClientRoutes.RegistrationRedirectRoute);
                return Task.CompletedTask;
            }

            _userRegistrationViewService.SelectProvider(provider);

            if (provider.CanProvideEmail)
                NavigationService.NavigateTo(ClientRoutes.RegistrationEmailRoute);
            else
                NavigationService.NavigateTo(ClientRoutes.RegistrationPhoneRoute);

            return Task.CompletedTask;
        }

        public void SetProviderError(Guid channelGuid)
        {
            ViewState.HasError = true;
            ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_PROVIDER_REDIRECT_NOT_IMPLEMENTED));
            ViewState.FailedChannelGuid = channelGuid;
            ViewState.RaiseChanged();
        }

        public Task NavigateBackAsync()
        {
            NavigationService.NavigateTo(ClientRoutes.RegistrationIndexRoute);
            return Task.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                var providers = await _registrationService.GetProvidersAsync(cancellationToken);
                ViewState.Providers = providers;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load registration providers.");
                ViewState.HasError = true;
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        #endregion
    }
}
