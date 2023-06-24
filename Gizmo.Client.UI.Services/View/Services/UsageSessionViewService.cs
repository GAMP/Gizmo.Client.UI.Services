using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UsageSessionViewService : ViewStateServiceBase<UsageSessionViewState>
    {
        public UsageSessionViewService(UsageSessionViewState viewState,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,            
            ILogger<UsageSessionViewService> logger,
            IServiceProvider serviceProvider)
            :base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.UsageSessionChange += OnUsageSessionChange;
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
            _gizmoClient.UsageSessionChange -= OnUsageSessionChange;
            _gizmoClient.LoginStateChange -= OnLoginStateChange;

            base.OnDisposing(isDisposing);
        }

        private void OnUsageSessionChange(object? sender, UsageSessionChangedEventArgs e)
        {
            switch(e.CurrentUsageType)
            {
                case UsageType.Rate:
                    ViewState.CurrentTimeProduct = _localizationService.GetString("GIZ_USAGE_TYPE_RATE");
                    break;
                case UsageType.TimeFixed:
                case UsageType.TimeOffer:
                    ViewState.CurrentTimeProduct = e.CurrentTimeProduct;
                    break;
            }
            
            DebounceViewStateChanged();
        }
    }
}
