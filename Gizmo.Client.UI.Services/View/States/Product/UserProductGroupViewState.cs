using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// User product group view state.
    /// </summary>
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserProductGroupViewState : UserProductViewStateBase
    {
    }
}
