using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class ApplicationGroupViewState : ViewStateBase
    {
        #region FIELDS
        private int _id;
        private string _name = string.Empty;
        #endregion

        #region PROPERTIES

        public int Id
        {
            get { return _id; }
            internal set { SetProperty(ref _id, value); }
        }

        public string Name
        {
            get { return _name; }
            internal set { SetProperty(ref _name, value); }
        }

        #endregion
    }
}
