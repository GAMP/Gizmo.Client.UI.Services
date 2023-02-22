using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// User product view state.
    /// </summary>
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserProductViewState : UserProductViewStateBase
    {
    }
}
