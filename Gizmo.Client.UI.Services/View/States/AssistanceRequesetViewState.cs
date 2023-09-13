using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Assistance request view state.
    /// </summary>
    [Register()]
    public sealed class AssistanceRequesetViewState : ValidatingViewStateBase
    {
        public IEnumerable<AssistanceRequestTypeViewState> AssistanceRequestTypes { get; internal set; } = Enumerable.Empty<AssistanceRequestTypeViewState>();

        [ValidatingProperty()]
        public int? SelectedAssistanceRequestType { get; internal set; }

        public string? Note { get; internal set; } = null!;

        public bool AnyPending { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;
    }
}
