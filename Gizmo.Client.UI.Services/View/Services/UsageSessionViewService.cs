using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UsageSessionViewService : ViewStateServiceBase<UsageSessionViewState>
    {
        public UsageSessionViewService(UsageSessionViewState viewState, 
            IGizmoClient gizmoClient,            
            ILogger<UsageSessionViewService> logger,
            IServiceProvider serviceProvider)
            :base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.UsageSessionChanged += OnUsageSessionChange;
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            return base.OnInitializing(ct);
        }

        private void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            switch(e.State)
            {
                case LoginState.LoggedOut:
                    break;
                default:
                    break;
            }
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.UsageSessionChanged -= OnUsageSessionChange;
            _gizmoClient.LoginStateChange -= OnLoginStateChange;

            base.OnDisposing(isDisposing);
        }

        private void OnUsageSessionChange(object? sender, UsageSessionChangedEventArgs e)
        {
            switch(e.CurrentUsageType)
            {
                case UsageType.Rate:
                    ViewState.CurrentTimeProduct = "Rate";
                    break;
                case UsageType.TimeOffer:
                    ViewState.CurrentTimeProduct = e.CurrentTimeProduct;
                    break;
                case UsageType.TimeFixed:
                    ViewState.CurrentTimeProduct = "Fixed time";
                    break;
            }
            
            DebounceViewStateChanged();
        }
    }
}
