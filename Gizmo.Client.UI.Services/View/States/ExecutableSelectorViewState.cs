using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ExecutableSelectorViewState : ViewStateBase
    {
        #region FIELDS
        private ApplicationViewState _application = new();
        #endregion

        #region PROPERTIES

        public ApplicationViewState Application
        {
            get { return _application; }
            internal set { _application = value; }
        }

        #endregion
    }
}
