using System.ComponentModel.DataAnnotations;
using System.Security;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserOnlineDepositViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        public bool IsEnabled { get; internal set; }

        public int PageIndex { get; internal set; }

        public IEnumerable<decimal> Presets { get; internal set; } = Enumerable.Empty<decimal>();

        public bool AllowCustomValue { get; internal set; }

        public decimal MinimumAmount { get; internal set; }

        [ValidatingProperty()]
        [Required(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_REQUIRED_FIELD")]
        public decimal? Amount { get; internal set; }

        [ValidatingProperty()]
        [Required(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_REQUIRED_FIELD")]
        public int? SelectedPaymentMethodId { get; internal set; }

        public string PaymentUrl { get; internal set; } = string.Empty;

        public string QrImage { get; internal set; } = string.Empty;

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;


        #endregion
    }
}
