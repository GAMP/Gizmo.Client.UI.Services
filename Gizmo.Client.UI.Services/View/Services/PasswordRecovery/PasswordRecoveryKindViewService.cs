using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    [Route(ClientRoutes.PasswordRecoveryKindRoute)]
    public sealed class PasswordRecoveryKindViewService : ViewStateServiceBase<PasswordRecoveryKindViewState>
    {
        private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly IPasswordRecoverySessionService _session;

        public PasswordRecoveryKindViewService(
            PasswordRecoveryKindViewState viewState,
            ILogger<PasswordRecoveryKindViewService> logger,
            IServiceProvider serviceProvider,
            IPasswordRecoveryService passwordRecoveryService,
            IPasswordRecoverySessionService session) : base(viewState, logger, serviceProvider)
        {
            _passwordRecoveryService = passwordRecoveryService;
            _session = session;
        }

        public void SelectKind(PasswordRecoveryIdentifierKind kind)
        {
            _session.SetIdentifierKind(kind);
            NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryDestinationRoute);
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            _session.Clear();

            ViewState.IsLoading = true;
            ViewState.CanUseEmail = false;
            ViewState.CanUseMobilePhone = false;
            ViewState.RaiseChanged();

            IReadOnlyList<PasswordRecoveryProvider> methods;

            try
            {
                methods = await _passwordRecoveryService.GetConfiguredMethodsAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Password recovery configured methods load error on navigate-in.");
                NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                return;
            }

            if (methods.Count == 0)
            {
                NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                return;
            }

            ViewState.CanUseEmail = methods.Any(m => m.Channel == PasswordRecoveryChannel.Email);
            ViewState.CanUseMobilePhone = methods.Any(m => m.Channel == PasswordRecoveryChannel.Sms);
            ViewState.IsLoading = false;
            ViewState.RaiseChanged();
        }
    }
}
