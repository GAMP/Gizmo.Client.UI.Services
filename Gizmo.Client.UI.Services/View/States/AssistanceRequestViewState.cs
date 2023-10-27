using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Assistance request view state.
    /// </summary>
    [Register()]
    public sealed class AssistanceRequestViewState : ValidatingViewStateBase
    {
        public bool IsEnabled { get; internal set; }

        public IEnumerable<AssistanceRequestTypeViewState> AssistanceRequestTypes { get; internal set; } = Enumerable.Empty<AssistanceRequestTypeViewState>();

        public int NoteLimit { get; internal set; } = 200;

        [ValidatingProperty()]
        public int? SelectedAssistanceRequestType { get; internal set; }

        public string? Note { get; internal set; } = null!;

        public bool AnyPending { get; internal set; }

        public bool IsLoading { get; internal set; }
    }
}
