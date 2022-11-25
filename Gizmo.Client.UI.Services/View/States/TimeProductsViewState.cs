using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class TimeProductsViewState : ViewStateBase
    {
        #region FIELDS
        private List<TimeProductViewState> _timeProducts = new();
        #endregion

        #region PROPERTIES

        public List<TimeProductViewState> TimeProducts
        {
            get { return _timeProducts; }
            internal set { _timeProducts = value; }
        }

        #endregion
    }
}
