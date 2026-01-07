using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ClientServerCartViewState : ViewStateBase, ICartViewState
    {
        private readonly ConcurrentDictionary<Guid, UserCartProductViewState> _entries = [];

        public IEnumerable<UserCartProductViewState> Products => _entries.Values.OrderByDescending(line => line.Number);

        // line number counter, used to assign an unique number to each line in cart
        private long _lineNumber;

        /// <summary>
        /// Adds entry to user cart.
        /// </summary>
        /// <param name="state">Cart entry state.</param>
        /// <returns></returns>
        public UserCartProductViewState Add(UserCartProductViewState state)
        {
            return _entries.GetOrAdd(state.Guid, state);
        }

        public bool TryGetProductEntryViewState(Guid entryId, [NotNullWhen(true)] out UserCartProductViewState? viewState)
        {
            viewState = null;

            viewState = Products.Where(entry => entry.Guid == entryId).FirstOrDefault();

            return viewState != null;
        }

        public bool TryRemoveEntry(Guid entryId)
        {
            return _entries.Remove(entryId, out _);
        }

        public void Clear()
        {
            // reset line number counter
            _lineNumber = 0;

            _entries.Clear();

            //TODO: AAAAA ResetPromotionState();
        }

        public bool IsStateUpdateRequired { get; set; }

        public bool IsStateUpdating { get; internal set; }

        public int PointsTotal { get; internal set; }

        public decimal SubTotal { get; internal set; }

        public decimal TaxTotal { get; internal set; }

        public decimal FeeTotal { get; internal set; }

        public decimal Discount { get; internal set; }

        public decimal Total { get; internal set; }

        public int PointsAward { get; internal set; }

        public ICartPromoCodeViewState PromoCodeViewState => null; //TODO: AAAAA
    }
}
