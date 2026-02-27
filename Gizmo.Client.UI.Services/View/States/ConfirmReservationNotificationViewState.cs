using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ConfirmReservationNotificationViewState : ViewStateBase
    {
        #region PROPERTIES

        public int Step { get; internal set; }

        public bool IsLoading { get; internal set; }

        #endregion
    }
}
