﻿using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserLoginStatusViewService : ViewStateServiceBase<UserLoginStatusViewState>
    {
        public UserLoginStatusViewService(UserLoginStatusViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<UserLoginStatusViewService> logger,
            IServiceProvider serviceProvider,
            UserChangePasswordViewService userChangePasswordViewService,
            UserChangeProfileViewService userChangeProfileViewService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _userChangePasswordViewService = userChangePasswordViewService;
            _userChangeProfileViewService = userChangeProfileViewService;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly UserChangePasswordViewService _userChangePasswordViewService;
        private readonly UserChangeProfileViewService _userChangeProfileViewService;
        
        private const string LOGIN_ROUTE_URL = "https://0.0.0.0/";

        private bool _isLoggedIn = false;

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnUserLoginStateChange;

            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            base.OnDisposing(isDisposing);

            _gizmoClient.LoginStateChange += OnUserLoginStateChange;
        }

        private async void OnUserLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.LoginCompleted: // use login completed so the ui will only be unblocked when all login procedures have finished
                    ViewState.IsLoggedIn = true;
                    ViewState.Username = e.UserProfile?.UserName;
                    break;
                default:
                    ViewState.IsLoggedIn = false;
                    ViewState.Username = null;
                    break;
            }

            switch (e.State)
            {
                case LoginState.LoggingIn:
                case LoginState.LoggedIn:
                    _isLoggedIn = true;
                    break;
                case LoginState.LoggingOut:
                case LoginState.LoggedOut:
                    _isLoggedIn = false;
                    break;
                default: break;
            }

            switch (e.State)
            {
                case LoginState.LoginCompleted:
                    NavigationService.NavigateTo(ClientRoutes.HomeRoute);
                    break;
                case LoginState.LoggingOut:
                    NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                    break;
                default:
                    break;
            }

            try
            {
                if (e.State == LoginState.LoginCompleted && e.IsUserPasswordRequired)
                {
                    await _userChangePasswordViewService.StartAsync(false);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User password update failed during login process.");
            }

            try
            {
                if (e.State == LoginState.LoginCompleted && e.IsUserInfoRequired)
                {
                    await _userChangeProfileViewService.StartAsync();
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "User profile update failed during login process.");
            }           
        }

        protected override Task OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            try
            {
                //TODO temprary fix, we need to fix the mouse buttons probelem
                if (_isLoggedIn && e.Location == LOGIN_ROUTE_URL)
                {
                    NavigationService.NavigateTo(ClientRoutes.HomeRoute);
                }
                else if (!_isLoggedIn && e.Location != LOGIN_ROUTE_URL)
                {
                    NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                }                
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Error handling location change.");
            }

            return base.OnLocationChanged(sender, e);
        }
    }
}
