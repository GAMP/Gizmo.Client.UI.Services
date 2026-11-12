using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class LoginRotatorItemViewState : ViewStateBase
    {
        #region PROPERTIES

        public string MediaPath { get; internal set; } = string.Empty;

        public bool IsVideo { get; internal set; }

        /// <summary>
        /// Offset inside a video, in seconds. Pictures stay at 0.
        /// </summary>
        public double PositionSeconds { get; internal set; }

        #endregion
    }
}
