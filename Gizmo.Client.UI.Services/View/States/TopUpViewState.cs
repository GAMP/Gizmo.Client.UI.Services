using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class TopUpViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private bool _isLoading;
        private int _pageIndex;
        private IEnumerable<decimal> _presets = Enumerable.Empty<decimal>();
        private bool _allowCustomValue;
        private decimal _minimumAmount;
        private decimal _amount;
        #endregion

        #region PROPERTIES

        public bool IsLoading
        {
            get { return _isLoading; }
            internal set { _isLoading = value; }
        }

        public int PageIndex
        {
            get { return _pageIndex; }
            internal set { _pageIndex = value; }
        }

        public IEnumerable<decimal> Presets
        {
            get { return _presets; }
            internal set { _presets = value; }
        }

        public bool AllowCustomValue
        {
            get { return _allowCustomValue; }
            internal set { _allowCustomValue = value; }
        }

        public decimal MinimumAmount
        {
            get { return _minimumAmount; }
            internal set { _minimumAmount = value; }
        }

        [ValidatingProperty()]
        public decimal Amount
        {
            get { return _amount; }
            internal set { _amount = value; }
        }

        #endregion
    }
}
