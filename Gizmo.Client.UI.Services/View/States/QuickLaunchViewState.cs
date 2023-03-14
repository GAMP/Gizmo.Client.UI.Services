using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class QuickLaunchViewState : ViewStateBase
    {
        #region PROPERTIES

        public IEnumerable<AppExeViewState> Executables { get; internal set; } = Enumerable.Empty<AppExeViewState>();

        #endregion
    }
}
