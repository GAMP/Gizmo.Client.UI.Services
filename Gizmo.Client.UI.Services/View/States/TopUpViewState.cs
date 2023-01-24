﻿using Gizmo.UI;
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
        private List<decimal> _presets = new();
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

        public List<decimal> Presets
        {
            get { return _presets; }
            internal set { _presets = value; }
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