using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AssistanceRequestTypeViewState : ViewStateBase
    {
        public int Id { get; internal set; }

        public string Title { get; internal set; } = string.Empty;

        public int DisplayOrder { get; internal set; }

        public bool IsDeleted { get; internal set; }
    }
}
