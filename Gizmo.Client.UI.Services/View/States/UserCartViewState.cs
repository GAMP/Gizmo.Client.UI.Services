using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserCartViewState : ValidatingViewStateBase //TODO: AAAAA
    {
        #region PROPERTIES

        [ValidatingProperty()]
        public string? Notes { get; internal set; } = null!;

        public bool ShowPaymentMethods { get; internal set; }

        [ValidatingProperty()]
        public int? PaymentMethodId { get; internal set; } = null!;

        public bool IsComplete { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
