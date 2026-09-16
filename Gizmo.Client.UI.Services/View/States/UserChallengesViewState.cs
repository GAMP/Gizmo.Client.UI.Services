using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserChallengesViewState : ViewStateBase
    {
        public IEnumerable<UserChallengeViewState> Challenges { get; internal set; } = Enumerable.Empty<UserChallengeViewState>();
        /// <summary>Header counter text, e.g. "1 completed".</summary>
        public string CompletedCountText { get; internal set; } = string.Empty;
        public bool IsLoading { get; internal set; }
        public bool HasError { get; internal set; }
        public string ErrorMessage { get; internal set; } = string.Empty;
    }
}
