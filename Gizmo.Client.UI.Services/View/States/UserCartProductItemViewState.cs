using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserCartProductItemViewState : ViewStateBase
    {
        public int ProductId { get; internal set; }
        public int Quantity { get; internal set; }
    }
}
