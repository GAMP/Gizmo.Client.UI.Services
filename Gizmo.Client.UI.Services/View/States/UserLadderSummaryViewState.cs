using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserLadderSummaryViewState : ViewStateBase
    {
        public bool HasLevel { get; internal set; }
        public int Ordinal { get; internal set; }
        public string? EmblemUrl { get; internal set; }
        public string LevelName { get; internal set; } = string.Empty;
        public string HeaderStatusText { get; internal set; } = string.Empty;
        public string TopBarProgressText { get; internal set; } = string.Empty;
    }
}
