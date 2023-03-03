using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class TimeProductsViewState : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<TimeProductViewState> _timeProducts = Enumerable.Empty<TimeProductViewState>();
        #endregion

        #region PROPERTIES

        public IEnumerable<TimeProductViewState> TimeProducts
        {
            get { return _timeProducts; }
            internal set { _timeProducts = value; }
        }

        #endregion
    }
}
