using Gizmo.UI;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    public interface IClientNotificationService : INotificationsService
    {
        /// <summary>
        /// Shows alert notficiation.
        /// </summary>
        /// <param name="alertTypes">Alert type.</param>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="addOptions">Optional add options.</param>
        /// <param name="displayOptions">Optional display options.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Add result.</returns>
        Task<AddNotificationResult<EmptyComponentResult>> ShowAlertNotification(AlertTypes alertTypes,
            string title,
            string message,
            NotificationDisplayOptions? displayOptions = default,
            NotificationAddOptions? addOptions = default,
            CancellationToken cancellationToken = default);
    }
}
