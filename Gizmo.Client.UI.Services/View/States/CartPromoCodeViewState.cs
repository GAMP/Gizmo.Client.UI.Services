using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Singelton)]
    public sealed class CartPromoCodeViewState : ViewStateBase, ICartPromoCodeViewState
    {
        /// <summary>
        /// Current promo code id. Will be null if no promo code applied.
        /// </summary>
        public int? PromoCodeId { get; set; }

        /// <summary>
        /// Current promo code input.
        /// </summary>
        public string InputPromoCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets promo code value.
        /// </summary>
        /// <remarks>
        /// This will represent the current value of promo code example "WINTER26-SALE" etc.
        /// </remarks>
        public string? PromoCode { get; set; }

        /// <summary>
        /// Promotion name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Promotion description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Promotion discount names.
        /// </summary>
        public IEnumerable<string> DiscountNames { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Indicates that promocode is being applied.
        /// </summary>
        public bool IsLoading { get; set; }
    }
}
