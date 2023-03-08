using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ApplicationsPageViewState : ViewStateBase
    {
        #region PROPERTIES

        public IEnumerable<AppViewState> Applications { get; internal set; } = Enumerable.Empty<AppViewState>();

        #endregion
    }
}
