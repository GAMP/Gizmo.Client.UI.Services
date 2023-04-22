using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// User balance view state service.
    /// </summary>
    /// <remarks>
    /// Responsible of updating user balance view state.
    /// </remarks>
    [Register()]
    public sealed class UserBalanceViewService : ViewStateServiceBase<UserBalanceViewState>
    {
        public UserBalanceViewService(UserBalanceViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<UserBalanceViewService> logger,
            IServiceProvider serviceProvider):base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            _gizmoClient.UserBalanceChange += OnUserBalanceChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.LoginStateChange -= OnLoginStateChange;
            _gizmoClient.UserBalanceChange -= OnUserBalanceChange;
            base.OnDisposing(isDisposing);
        }

        private void OnUserBalanceChange(object? sender, UserBalanceEventArgs e)
        {
            ViewState.Balance = e.Balance.Balance;
            ViewState.PointsBalance = e.Balance.Points;
            ViewState.Outstanding = e.Balance.TotalOutstanding;
            ViewState.Time = TimeSpan.FromSeconds(e.Balance.AvailableCreditedTime ?? 0);
            DebounceViewStateChanged();
        }

        private async void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            //we only interested in user login/logout states
            switch (e.State)
            {
                case LoginState.LoggedIn:
                case LoginState.LoggedOut:
                    break;
                default:
                    return;
            }

            //we only need to update user balance
            if (e.State == LoginState.LoggedIn)
            {

                try
                {
                    //get current user balance
                    var currentUserBalance = await _gizmoClient.UserBalanceGetAsync();

                    //update values
                    ViewState.Balance = currentUserBalance.Balance;
                    ViewState.PointsBalance = currentUserBalance.Points;
                    ViewState.Outstanding = currentUserBalance.TotalOutstanding;
                    ViewState.Time = TimeSpan.FromSeconds(currentUserBalance.AvailableCreditedTime ?? 0);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed obtaining user balance.");
                }
            }
            else if (e.State == LoginState.LoggedOut)
            {
                ViewState.SetDefaults();
            }

            DebounceViewStateChanged();
        }
    }
}
