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

        #endregion
    }
}
