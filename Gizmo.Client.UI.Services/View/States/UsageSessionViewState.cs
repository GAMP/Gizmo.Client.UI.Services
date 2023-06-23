using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UsageSessionViewState : ViewStateBase
    {
        #region PROPERTIES

        public string CurrentTimeProduct { get; internal set; } = string.Empty;

        #endregion
    }
}
