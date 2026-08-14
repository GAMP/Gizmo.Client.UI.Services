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

        public async Task SelectProviderAsync(int methodId)
        {
            if (ViewState.IsLoading)
                return;

            var provider = ViewState.Providers.FirstOrDefault(p => p.MethodId == methodId);
            if (provider is null)
                return;

            var identifierKind = _session.IdentifierKind;
            if (identifierKind is null)
            {
                NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryKindRoute);
                return;
            }

            ViewState.IsLoading = true;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            _session.SetActiveProvider(provider);

            try
            {
                var matchValue = _session.MatchValue;

                var startResult = await _passwordRecoveryService.StartAsync(new PasswordRecoveryStartRequest
                {
                    MethodId = provider.MethodId,
                    IdentifierKind = identifierKind.Value,
                    Value = matchValue
                });

                var outcome = PasswordRecoveryStartOutcome.Apply(startResult, _session, _localizationService);

                switch (outcome)
                {
                    case PasswordRecoveryStartOutcome.Result.Started:
                        NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryConfirmationRoute);
                        break;

                    case PasswordRecoveryStartOutcome.Result.Failed failed:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = failed.Message;
                        ViewState.ShowAllProviders = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Password recovery start error.");
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
                ViewState.ShowAllProviders = true;
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            var failedProviderChannelGuid = _session.FailedProviderChannelGuid;
            var showAllProviders = _session.ShowAllProviders;
            _session.SetFailedProviderChannelGuid(null);
            _session.SetShowAllProviders(false);

            if (_session.IdentifierKind is null || _session.AvailableMethods.Count == 0)
            {
                NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryKindRoute);
                return Task.CompletedTask;
            }

            var hasError = failedProviderChannelGuid.HasValue;

            ViewState.HasError = hasError;
            ViewState.ErrorMessage = hasError
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RECOVERY_PASSWORD_RESET_FAILED_MESSAGE))
                : string.Empty;
            ViewState.ShowAllProviders = showAllProviders;
            ViewState.Providers = _session.AvailableMethods;
            ViewState.IsLoading = false;
            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }
    }
}
