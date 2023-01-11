using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserService : ViewStateServiceBase<UserViewState>, IDisposable
    {
        #region CONSTRUCTOR
        public UserService(UserViewState viewState,
            ILogger<UserService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion

        #region PROPERTIES

        public UserBalanceViewState UserBalanceViewState { get; set; } = new UserBalanceViewState()
        {
            Balance = 40.5m,
            PointsBalance = 450
        };

        #endregion

        #region FUNCTIONS

        public Task LogοutAsync()
        {
            NavigationService.NavigateTo(ClientRoutes.LoginRoute);
            return Task.CompletedTask;
        }

        #endregion
    }
}
