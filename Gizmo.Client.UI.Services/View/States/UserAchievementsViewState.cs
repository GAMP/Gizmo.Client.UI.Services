using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserAchievementsViewState : ViewStateBase
    {
        public IEnumerable<UserAchievementViewState> Achievements { get; internal set; } = Enumerable.Empty<UserAchievementViewState>();
        public bool IsLoading { get; internal set; }
        public bool HasError { get; internal set; }
        public string ErrorMessage { get; internal set; } = string.Empty;
    }
}
