using Gizmo.UI;

namespace Gizmo.Client.UI.Services
{
    public sealed class WebNotificationHost : INotificationsHost
    {
        public Task HideAsyc()
        {
            return Task.CompletedTask;
        }

        public Task ShowAsync()
        {
            return Task.CompletedTask;
        }
    }
}
