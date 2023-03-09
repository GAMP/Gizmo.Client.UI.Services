using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AppRatingViewState : ViewStateBase
    {
        #region PROPERTIES

        public int ApplicationId { get; internal set; }

        public int UserRating { get; internal set; }

        public int Rating { get; internal set; }

        public int TotalRates { get; internal set; }

        #endregion
    }
}
