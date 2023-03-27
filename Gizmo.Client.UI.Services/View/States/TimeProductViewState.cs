using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class TimeProductViewState : ViewStateBase
    {
        #region FIELDS
        private DateTime _purchaseDate;
        private string _title = string.Empty;
        private TimeSpan _time;
        #endregion

        #region PROPERTIES

        public DateTime PurchaseDate
        {
            get { return _purchaseDate; }
            internal set { _purchaseDate = value; }
        }

        public string Title
        {
            get { return _title; }
            internal set { _title = value; }
        }

        public TimeSpan Time
        {
            get { return _time; }
            internal set { _time = value; }
        }

        #endregion
    }
}
