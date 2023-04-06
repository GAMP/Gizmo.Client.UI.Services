using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class PaymentMethodViewState : ViewStateBase
    {
        public int Id { get; internal set; }
        public string Name { get; internal set; } = null!;
        public bool IsOnline { get; internal set; }
    }
}
