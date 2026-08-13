using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    [Route(ClientRoutes.PasswordRecoveryKindRoute)]
    public sealed class PasswordRecoveryKindViewService : ViewStateServiceBase<PasswordRecoveryKindViewState>
    {
        private readonly IPasswordRecoverySessionService _session;

        public PasswordRecoveryKindViewService(
            PasswordRecoveryKindViewState viewState,
            ILogger<PasswordRecoveryKindViewService> logger,
            IServiceProvider serviceProvider,
            IPasswordRecoverySessionService session) : base(viewState, logger, serviceProvider)
        {
            _session = session;
        }

        public void SelectKind(PasswordRecoveryIdentifierKind kind)
        {
            _session.SetIdentifierKind(kind);
            NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryDestinationRoute);
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            _session.Clear();
            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }
    }
}
