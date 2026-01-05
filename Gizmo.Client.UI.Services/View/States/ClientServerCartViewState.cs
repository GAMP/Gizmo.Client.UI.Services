using System.Diagnostics.CodeAnalysis;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ClientServerCartViewState : ViewStateBase, ICartViewState
    {
        private List<UserCartProductViewState> _products = new List<UserCartProductViewState>();

        public IEnumerable<UserCartProductViewState> Products => _products;

        // line number counter, used to assign an unique number to each line in cart
        private long _lineNumber;

        /// <summary>
        /// Adds entry to user cart.
        /// </summary>
        /// <param name="state">Cart entry state.</param>
        /// <returns></returns>
        public UserCartProductViewState Add(UserCartProductViewState state)
        {
            state.Number = Interlocked.Increment(ref _lineNumber);

            _products.Add(state);

            return state;
        }

        public bool TryGetProductEntryViewState(Guid entryId, [NotNullWhen(true)] out UserCartProductViewState? viewState)
        {
            viewState = null;

            viewState = Products.Where(entry => entry.Guid == entryId).FirstOrDefault();

            return viewState != null;
        }

        public bool TryRemoveEntry(Guid entryId)
        {
            var viewState = _products.Where(entry => entry.Guid == entryId).FirstOrDefault();

            if (viewState != null)
                _products.Remove(viewState);

            return viewState != null;
        }

        public bool IsStateUpdateRequired { get; set; }

        public bool IsStateUpdating => false;

        public int PointsTotal => 0;

        public decimal SubTotal => 0;

        public decimal TaxTotal => 0;

        public decimal FeeTotal => 0;

        public decimal Discount => 0;

        public decimal Total => 0;

        public int PointsAward => 0;

        public ICartPromoCodeViewState PromoCodeViewState => null;
    }
}
