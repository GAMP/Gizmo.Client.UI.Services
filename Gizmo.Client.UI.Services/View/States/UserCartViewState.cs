using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserCartViewState : ViewStateBase
    {
        #region FIELDS
        private decimal _total;
        private int _awardPoints;
        private readonly List<UserCartProductViewState> _products = new();
        #endregion

        #region PROPERTIES
        
        /// <summary>
        /// Gets current user cart product states.
        /// </summary>
        public IEnumerable<UserCartProductViewState> Products
        {
            get { return _products; }
        }

        /// <summary>
        /// Gets cart total.
        /// </summary>
        public decimal Total
        {
            get { return _total; }
            internal set { SetProperty(ref _total, value); }
        }

        /// <summary>
        /// Gets cart award points.
        /// </summary>
        public int AwardPoints
        {
            get { return _awardPoints; }
            internal set { SetProperty(ref _awardPoints, value); }
        } 

        #endregion
    }
}
