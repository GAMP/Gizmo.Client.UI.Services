using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class TimeProductsViewState : ViewStateBase
    {
        #region PROPERTIES

        public IEnumerable<TimeProductViewState> TimeProducts { get; internal set; } = Enumerable.Empty<TimeProductViewState>();

        public PaginationCursor? PrevCursor { get; internal set; }

        public PaginationCursor? NextCursor { get; internal set; }

        #endregion
    }
}
