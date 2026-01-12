using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Singelton)]
    public sealed class ClientServerCartViewState : ViewStateBase, ICartViewState
    {
        public ClientServerCartViewState(CartPromoCodeViewState cartPromoCodeViewState)
        {
            _promoCodeViewState = cartPromoCodeViewState;
        }

        private readonly ConcurrentDictionary<Guid, UserCartProductViewState> _entries = [];
        private readonly CartPromoCodeViewState _promoCodeViewState;

        // line number counter, used to assign an unique number to each line in cart
        private long _lineNumber;

        public IEnumerable<UserCartProductViewState> Products => _entries.Values.OrderByDescending(line => line.Number);

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

            IsStateUpdateRequired = false;
            IsStateUpdating = false;
            PointsTotal = 0;
            SubTotal = 0;
            TaxTotal = 0;
            FeeTotal = 0;
            Discount = 0;
            Total = 0;
            PointsAward = 0;
            PromotionExceptionMessage = null;

            ResetPromotionState();
        }

        /// <summary>
        /// Reset promotion state to default values.
        /// </summary>
        public void ResetPromotionState()
        {
            // reset promotion code loading state
            PromoCodeViewState.IsLoading = false;

            // reset promo code state
            PromoCodeViewState.PromoCodeId = null;
            PromoCodeViewState.InputPromoCode = string.Empty;
            PromoCodeViewState.PromoCode = null;
            PromoCodeViewState.Description = null;
            PromoCodeViewState.Name = null;
            PromoCodeViewState.DiscountNames = Enumerable.Empty<string>();

            PromoCodeViewState.RaiseChanged();
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

        public PromoCodeApplyStatus PromoCodeStatus { get; internal set; }

        public string? PromotionExceptionMessage { get; set; }

        public ICartPromoCodeViewState PromoCodeViewState => _promoCodeViewState;
    }
}
