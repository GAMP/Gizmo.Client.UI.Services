using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Host group view state.
    /// </summary>
    [Register()]
    public sealed class HostGroupViewState : ViewStateBase
    {
        #region PROPERTIES
        public int? HostGroupId { get; internal set; }
        #endregion
    }
}
