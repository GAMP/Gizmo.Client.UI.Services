using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// User balance view state service.
    /// </summary>
    /// <remarks>
    /// Responsible of updating user balance view state.
    /// </remarks>
    [Register()]
    public sealed class UserBalanceTooltipViewService : ViewStateServiceBase<UserBalanceTooltipViewState>
    {
        public UserBalanceTooltipViewService(UserBalanceTooltipViewState viewState,
            ILogger<UserBalanceViewService> logger,
            IServiceProvider serviceProvider,
            IOptions<ClientShopOptions> shopOptions,
            IOptions<UserOnlineDepositOptions> userOnlineDepositOptions) : base(viewState, logger, serviceProvider)
        {
            _shopOptions = shopOptions;
            _userOnlineDepositOptions = userOnlineDepositOptions;
        }

        private readonly IOptions<ClientShopOptions> _shopOptions;
        private readonly IOptions<UserOnlineDepositOptions> _userOnlineDepositOptions;

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.DisableShop = _shopOptions.Value.Disabled;
            ViewState.DisableOnlineDeposits = _userOnlineDepositOptions.Value.Disabled;
            return base.OnInitializing(ct);
        }
    }
}
