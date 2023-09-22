using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserMenuViewService : ViewStateServiceBase<UserMenuViewState>
    {
        public UserMenuViewService(UserMenuViewState viewState,
            ILogger<UserMenuViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        public void ToggleUserOnlineDeposit()
        {
            ViewState.UserOnlineDepositIsVisible = !ViewState.UserOnlineDepositIsVisible;

            if (ViewState.UserOnlineDepositIsVisible)
            {
                ViewState.ActiveAppsIsVisible = false;
                ViewState.NotificationsIsVisible = false;
                ViewState.UserLinksIsVisible = false;
            }

            ViewState.RaiseChanged();
        }

        public void OpenUserOnlineDeposit()
        {
            ViewState.UserOnlineDepositIsVisible = true;

            if (ViewState.UserOnlineDepositIsVisible)
            {
                ViewState.ActiveAppsIsVisible = false;
                ViewState.NotificationsIsVisible = false;
                ViewState.UserLinksIsVisible = false;
            }

            ViewState.RaiseChanged();
        }

        public void CloseUserOnlineDeposit()
        {
            ViewState.UserOnlineDepositIsVisible = false;

            ViewState.RaiseChanged();
        }

        public void ToggleActiveApps()
        {
            ViewState.ActiveAppsIsVisible = !ViewState.ActiveAppsIsVisible;

            if (ViewState.ActiveAppsIsVisible)
            {
                ViewState.UserOnlineDepositIsVisible = false;
                ViewState.NotificationsIsVisible = false;
                ViewState.UserLinksIsVisible = false;
            }

            ViewState.RaiseChanged();
        }

        public void CloseActiveApps()
        {
            ViewState.ActiveAppsIsVisible = false;

            ViewState.RaiseChanged();
        }

        public void ToggleNotifications()
        {
            ViewState.NotificationsIsVisible = !ViewState.NotificationsIsVisible;

            if (ViewState.NotificationsIsVisible)
            {
                ViewState.UserOnlineDepositIsVisible = false;
                ViewState.ActiveAppsIsVisible = false;
                ViewState.UserLinksIsVisible = false;
            }

            ViewState.RaiseChanged();
        }

        public void CloseNotifications()
        {
            ViewState.NotificationsIsVisible = false;

            ViewState.RaiseChanged();
        }

        public void ToggleUserLinks()
        {
            ViewState.UserLinksIsVisible = !ViewState.UserLinksIsVisible;

            if (ViewState.UserLinksIsVisible)
            {
                ViewState.UserOnlineDepositIsVisible = false;
                ViewState.ActiveAppsIsVisible = false;
                ViewState.NotificationsIsVisible = false;
            }

            ViewState.RaiseChanged();
        }

        public void CloseUserLinks()
        {
            ViewState.UserLinksIsVisible = false;

            ViewState.RaiseChanged();
        }
    }
}
