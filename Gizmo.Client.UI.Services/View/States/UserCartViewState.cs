﻿using System.ComponentModel.DataAnnotations;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserCartViewState : ValidatingViewStateBase
    {
        #region FIELDS
        //private decimal _total;
        //private int _pointsAward;
        private string _notes = string.Empty;
        private int? _paymentMethodId;
        private bool _isLoading;
        private bool _isComplete;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets current user cart product states.
        /// </summary>
        public IEnumerable<UserCartProductItemViewState> Products { get; internal set; } = Enumerable.Empty<UserCartProductItemViewState>();

        [ValidatingProperty()]
        public string Notes
        {
            get { return _notes; }
            internal set { SetProperty(ref _notes, value); }
        }

        [ValidatingProperty()]
        [Required()]
        public int? PaymentMethodId
        {
            get { return _paymentMethodId; }
            internal set { SetProperty(ref _paymentMethodId, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            internal set { _isLoading = value; }
        }

        public bool IsComplete
        {
            get { return _isComplete; }
            internal set { SetProperty(ref _isComplete, value); }
        }

        #endregion
    }
}
