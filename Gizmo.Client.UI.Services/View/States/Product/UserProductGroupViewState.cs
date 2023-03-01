using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// User product group view state.
    /// </summary>
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserProductGroupViewState : ViewStateBase
    {
        /// <summary>
        /// Gets product group id.
        /// </summary>
        public int ProductGroupId { get; internal set; }

        /// <summary>
        /// Gets product group name.
        /// </summary>
        public string Name { get; internal set; } = null!;
    }
}
