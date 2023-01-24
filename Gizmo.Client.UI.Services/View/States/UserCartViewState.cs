using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserCartViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private readonly List<UserCartProductViewState> _products = new();
        //private decimal _total;
        //private int _pointsAward;
        private string _notes = string.Empty;
        private int? _paymentMethodId;
        private bool _isLoading;
        private bool _isComplete;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets current user cart product states.
        /// </summary>
        public List<UserCartProductViewState> Products
        {
            get { return _products; }
        }

        /// <summary>
        /// Gets cart total.
        /// </summary>
        public decimal Total
        {
            get { return _products.Where(a => a.PayType == OrderLinePayType.Cash || a.PayType == OrderLinePayType.Mixed).Select(a => a.UnitPrice * a.Quantity).Sum(); }
            //internal set { SetProperty(ref _total, value); }
        }

        /// <summary>
        /// Gets cart points total.
        /// </summary>
        public int PointsTotal
        {
            get { return _products.Where(a => a.PayType == OrderLinePayType.Points || a.PayType == OrderLinePayType.Mixed).Select(a => (a.UnitPointsPrice ?? 0) * a.Quantity).Sum(); }
        }

        /// <summary>
        /// Gets cart points award.
        /// </summary>
        public int PointsAward
        {
            get { return _products.Select(a => (a.UnitPointsAward ?? 0) * a.Quantity).Sum(); }
            //internal set { SetProperty(ref _pointsAward, value); }
        }

        [ValidatingProperty()]
        public string Notes
        {
            get { return _notes; }
            internal set { SetProperty(ref _notes, value); }
        }

        [ValidatingProperty()]
        [Required()]
        public int? PaymentMethodId
        {
            get { return _paymentMethodId; }
            internal set { SetProperty(ref _paymentMethodId, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            internal set { _isLoading = value; }
        }

        public bool IsComplete
        {
            get { return _isComplete; }
            internal set { SetProperty(ref _isComplete, value); }
        }

        #endregion
    }
}