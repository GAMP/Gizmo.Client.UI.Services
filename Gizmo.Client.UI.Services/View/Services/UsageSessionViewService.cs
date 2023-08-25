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

        private async void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            //we only need to update user session
            if (e.State == LoginState.LoggedIn)
            {
                try
                {
                    //get current user balance
                    var currentUserSession = await _gizmoClient.UserUsageSessionGetAsync();

                    ViewState.CurrentTimeProductType = currentUserSession.CurrentUsageType;
                    switch (currentUserSession.CurrentUsageType)
                    {
                        case UsageType.Rate:
                            ViewState.CurrentTimeProductName = _localizationService.GetString("GIZ_USAGE_TYPE_RATE");
                            break;
                        case UsageType.TimeFixed:
                        case UsageType.TimeOffer:
                            ViewState.CurrentTimeProductName = currentUserSession.TimePorduct;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed obtaining user session.");
                }
            }
            else if (e.State == LoginState.LoggedOut)
            {
                ViewState.SetDefaults();
            }

            DebounceViewStateChanged();
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.UsageSessionChange -= OnUsageSessionChange;
            _gizmoClient.LoginStateChange -= OnLoginStateChange;

            base.OnDisposing(isDisposing);
        }

        private void OnUsageSessionChange(object? sender, UsageSessionChangeEventArgs e)
        {
            ViewState.CurrentTimeProductType = e.CurrentUsageType;
            switch (e.CurrentUsageType)
            {
                case UsageType.Rate:
                    ViewState.CurrentTimeProductName = _localizationService.GetString("GIZ_USAGE_TYPE_RATE");
                    break;
                case UsageType.TimeFixed:
                case UsageType.TimeOffer:
                    ViewState.CurrentTimeProductName = e.CurrentTimeProduct;
                    break;
            }
            
            DebounceViewStateChanged();
        }
    }
}
