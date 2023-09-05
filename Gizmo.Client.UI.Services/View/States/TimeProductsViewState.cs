using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class TimeProductsViewState : ViewStateBase
    {
        #region PROPERTIES

        public IEnumerable<TimeProductViewState> TimeProducts { get; internal set; } = Enumerable.Empty<TimeProductViewState>();

        #endregion

        public override void SetDefaults()
        {
            TimeProducts = Enumerable.Empty<TimeProductViewState>();
        }
    }
}
