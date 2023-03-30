using System.Threading;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public sealed class UserPasswordRecoveryMethodServiceViewStateService : ViewStateServiceBase<UserPasswordRecoveryMethodServiceViewState>
    {
        #region CONTRUCTOR
        public UserPasswordRecoveryMethodServiceViewStateService(UserPasswordRecoveryMethodServiceViewState viewState,
            ILogger<UserPasswordRecoveryMethodServiceViewStateService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region OVERRIDES

        protected override async Task OnInitializing(CancellationToken ct)
        {
            ViewState.AvailabledRecoveryMethod = await _gizmoClient.GetPasswordRecoveryMethodAsync(ct);

            await base.OnInitializing(ct);
        }

        #endregion
    }
}
