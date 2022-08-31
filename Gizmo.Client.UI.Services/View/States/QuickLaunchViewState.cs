using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class QuickLaunchViewState : ViewStateBase
    {
        #region FIELDS
        private List<ExecutableViewState> _executables = new();
        #endregion

        #region PROPERTIES

        public List<ExecutableViewState> Executables
        {
            get { return _executables; }
            internal set { _executables = value; }
        }

        #endregion
    }
}
