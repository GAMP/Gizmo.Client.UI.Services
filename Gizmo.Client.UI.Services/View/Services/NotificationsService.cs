using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class NotificationsService : ViewStateServiceBase<NotificationsViewState>
    {
        #region CONSTRUCTOR
        public NotificationsService(NotificationsViewState viewState,
            ILogger<NotificationsService> logger,
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
                ViewState.Notifications.Remove(existingNotification);
            }
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }

        public Task MarkAllAsReadAsync()
        {
            ViewState.Notifications.Clear();
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }

        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            ViewState.Notifications = Enumerable.Range(1, 13).Select(i => new NotificationViewState()
            {
                Id = i,
                Time = "1 hour ago",
                Title = $"Order on hold {i}",
                Message = "Your order is on hold. You will be further be notified once order is accepted." //Order #0075364 was successfuly paid from your account.
            }).ToList();
        }
    }
}