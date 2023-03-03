using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserProductBundledViewState : UserProductViewStateBase
    {
        public decimal Quantity { get; internal set; }
    }
}
