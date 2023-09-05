using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class CreditOptionsViewService : ViewStateServiceBase<CreditOptionsViewState>
    {
        public CreditOptionsViewService(CreditOptionsViewState viewState,
            ILogger<FeedsViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        private async void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            if (e.State == LoginState.LoggedIn)
            {
                try
                {
                    var creditLimit = await _gizmoClient.UserCreditLimitGetAsync();

                    ViewState.SalesCreditType = creditLimit.SalesCreditType;
                    ViewState.TimeCreditType = creditLimit.TimeCreditType;
                    ViewState.CreditLimit = creditLimit.CreditLimit;
                    ViewState.IsTimeCreditEnabledByDefault = creditLimit.IsTimeCreditEnabledByDefault;
                    ViewState.IsUserTimeCreditEnabled = creditLimit.IsUserTimeCreditEnabled;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to obtain user credit limit.");
                }
            }
            else if (e.State == LoginState.LoggedOut)
            {
                ViewState.SetDefaults();
            }

            DebounceViewStateChanged();
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.LoginStateChange -= OnLoginStateChange;

            base.OnDisposing(isDisposing);
        }
    }
}
