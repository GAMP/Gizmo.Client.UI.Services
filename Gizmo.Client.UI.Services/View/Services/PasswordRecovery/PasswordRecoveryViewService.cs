using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public sealed class PasswordRecoveryViewService : ViewStateServiceBase<PasswordRecoveryViewState>
    {
        private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly IPasswordRecoverySessionService _session;
        private readonly ILocalizationService _localizationService;

        public PasswordRecoveryViewService(
            PasswordRecoveryViewState viewState,
            ILogger<PasswordRecoveryViewService> logger,
            IServiceProvider serviceProvider,
            IPasswordRecoveryService passwordRecoveryService,
            IPasswordRecoverySessionService session,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _passwordRecoveryService = passwordRecoveryService;
            _session = session;
            _localizationService = localizationService;
        }

        public Task SelectProviderAsync(Guid publicId)
        {
            var provider = ViewState.Providers.FirstOrDefault(p => p.PublicId == publicId);
            if (provider is null)
                return Task.CompletedTask;

            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            _session.SetActiveProvider(provider);
            NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryDestinationRoute);

            return Task.CompletedTask;
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            var failedProviderChannelGuid = _session.FailedProviderChannelGuid;
            var showAllProviders = _session.ShowAllProviders;
            _session.SetFailedProviderChannelGuid(null);
            _session.SetShowAllProviders(false);

            var hasError = failedProviderChannelGuid.HasValue;

            ViewState.HasError = hasError;
            ViewState.ErrorMessage = hasError
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RECOVERY_PASSWORD_RESET_FAILED_MESSAGE))
                : string.Empty;
            ViewState.ShowAllProviders = showAllProviders;
            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            IReadOnlyList<PasswordRecoveryProvider> providers;

            try
            {
                providers = await _passwordRecoveryService.GetProvidersAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Password recovery providers load error on navigate-in.");
                NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                return;
            }

            if (providers.Count == 0)
            {
                NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                return;
            }

            ViewState.Providers = providers;
            ViewState.IsLoading = false;
            ViewState.RaiseChanged();
        }
    }
}
