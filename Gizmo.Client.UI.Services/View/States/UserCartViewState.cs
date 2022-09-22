using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserCartViewState : ViewStateBase
    {
        #region FIELDS
        //private decimal _total;
        //private int _pointsAward;
        private readonly List<UserCartProductViewState> _products = new();
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

        #endregion
    }
}