using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class NotificationsViewService : ViewStateServiceBase<NotificationsViewState>
    {
        #region CONSTRUCTOR
        public NotificationsViewService(NotificationsViewState viewState,
            ILogger<NotificationsViewService> logger,
            IServiceProvider serviceProvider, IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public Task MarkAsReadAsync(int id)
        {
            var existingNotification = ViewState.Notifications.Where(a => a.Id == id).FirstOrDefault();
            if (existingNotification != null)
            {
                ViewState.Notifications = ViewState.Notifications.Where(a => a != existingNotification).ToList();
            }
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }

        public Task MarkAllAsReadAsync()
        {
            ViewState.Notifications = Enumerable.Empty<NotificationViewState>();
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }

        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            //TODO: A Hook for notifications?

            //Test
            ViewState.Notifications = Enumerable.Range(1, 13).Select(i => new NotificationViewState()
            {
                Id = i,
                Time = "1 hour ago",
                Title = $"Order on hold {i}",
                Message = "Your order is on hold. You will be further be notified once order is accepted." //Order #0075364 was successfuly paid from your account.
            }).ToList();
            //End Test
        }
    }
}
