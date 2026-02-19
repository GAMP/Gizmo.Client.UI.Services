using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class ConfirmReservationDialogViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        public int Step { get; internal set; }

        [ValidatingProperty()]
        public string? Pin { get; internal set; }

        [ValidatingProperty()]
        public int? PaymentMethodId { get; internal set; }

        public bool IsLoading { get; internal set; }

        #endregion
    }
}
