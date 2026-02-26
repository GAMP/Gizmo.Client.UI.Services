using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ConfirmReservationNotificationViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        public int Step { get; internal set; }

        [ValidatingProperty]
        public string? Pin { get; internal set; }

        public bool IsLoading { get; internal set; }

        public string? ErrorMessage { get; internal set; }

        #endregion
    }
}
