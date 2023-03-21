using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserBalanceService : ViewStateServiceBase<UserBalanceViewState>, IDisposable
    {
        #region CONSTRUCTOR
        public UserBalanceService(UserBalanceViewState viewState,
            ILogger<UserBalanceService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            //TODO: A DEMO
            ViewState.Balance = 40.5m;
            ViewState.PointsBalance = 450;
        }
        #endregion
    }
}
