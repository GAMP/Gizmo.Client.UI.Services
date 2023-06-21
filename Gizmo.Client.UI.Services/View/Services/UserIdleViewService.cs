using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserIdleViewService : ViewStateServiceBase<UserIdleViewState>
    {
        public UserIdleViewService(UserIdleViewState viewState, IGizmoClient gizmoClient, ILogger<UserIdleViewService> logger, IServiceProvider serviceProvider)
            :base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.UserIdleChange += OnUserIdleChange;
            return base.OnInitializing(ct);
        }     

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.UserIdleChange -= OnUserIdleChange;
            base.OnDisposing(isDisposing);
        }

        private void OnUserIdleChange(object? sender, UserIdleEventArgs e)
        {
            ViewState.IsIdle = e.IsIdle;
            DebounceViewStateChanged();
        }

        public void Toggle()
        {
            ViewState.IsIdle = !ViewState.IsIdle;
            DebounceViewStateChanged();
        }
    }
}
