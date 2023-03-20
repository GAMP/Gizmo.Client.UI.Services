﻿using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserService : ViewStateServiceBase<UserViewState>, IDisposable
    {
        #region CONSTRUCTOR
        public UserService(UserViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<UserService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        private readonly IGizmoClient _gizmoClient;

        #region PROPERTIES

        public UserBalanceViewState UserBalanceViewState { get; set; } = new UserBalanceViewState()
        {
            Balance = 40.5m,
            PointsBalance = 450
        };

        #endregion

        #region FUNCTIONS

        public async Task LogοutAsync()
        {
            try
            {
                await _gizmoClient.UserLogoutAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User initiated logout failed.");
            }
        }

        #endregion
    }
}
