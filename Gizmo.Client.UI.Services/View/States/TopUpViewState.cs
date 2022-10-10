using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class TopUpViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private int _pageIndex;
        private List<decimal> _presets = new();
        private decimal _amount;
        #endregion

        #region PROPERTIES

        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        public List<decimal> Presets
        {
            get { return _presets; }
            internal set { _presets = value; }
        }

        [ValidatingProperty()]
        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        #endregion
    }
}