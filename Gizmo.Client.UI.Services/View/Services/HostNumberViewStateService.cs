using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class HostNumberViewStateService : ViewStateServiceBase<HostNumberViewState>
    {
        #region CONSTRUCTOR
        public HostNumberViewStateService(HostNumberViewState viewState,
            ILogger<HostNumberViewStateService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        public HostConfigurationViewState HostConfigurationViewState { get; set; } = new HostConfigurationViewState()
        {
            //Test
            CanSignIn = true,
            CanSignInWithQR = true,
            CanSignUp = true
            //End Test
        };

        public ReservationViewState ReservationViewState { get; set; } = new ReservationViewState()
        {
            //Test
            IsPending = true
            //End Test
        };

        #endregion

        #region FUNCTIONS

        public Task SetHostLockStateAsync(bool value)
        {
            //Test
            ViewState.IsLocked = value;
            //End Test

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion
    }
}
