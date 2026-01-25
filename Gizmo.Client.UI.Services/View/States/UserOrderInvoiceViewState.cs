using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserOrderInvoiceViewState : ViewStateBase
    {
        #region PROPERTIES

        public int Id { get; internal set; }

        public Web.Api.Models.InvoiceStatus PaymentStatus { get; internal set; }

        public string PaymentMethodNames { get; internal set; } = string.Empty;

        public bool IsVoided { get; internal set; }

        #endregion
    }
}
