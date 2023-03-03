using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class QuickLaunchViewState : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<ExecutableViewState> _executables = Enumerable.Empty<ExecutableViewState>();
        #endregion

        #region PROPERTIES

        public IEnumerable<ExecutableViewState> Executables
        {
            get { return _executables; }
            internal set { _executables = value; }
        }

        #endregion
    }
}
