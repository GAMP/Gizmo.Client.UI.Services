using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class TimeProductViewState : ViewStateBase
    {
        #region FIELDS
        private string _title = string.Empty;
        #endregion

        #region PROPERTIES

        public string Title
        {
            get { return _title; }
            internal set { SetProperty(ref _title, value); }
        }

        #endregion
    }
}
