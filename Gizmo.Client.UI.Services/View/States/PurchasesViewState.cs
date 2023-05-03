using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class PurchasesViewState : ViewStateBase
    {
        #region PROPERTIES

        public IEnumerable<UserOrderViewState> Orders { get; internal set; } = Enumerable.Empty<UserOrderViewState>();

        public PaginationCursor? PrevCursor { get; internal set; }

        public PaginationCursor? NextCursor { get; internal set; }

        #endregion
    }
}
