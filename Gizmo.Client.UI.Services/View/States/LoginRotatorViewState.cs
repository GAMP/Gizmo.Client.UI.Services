using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class LoginRotatorViewState : ViewStateBase
    {
        #region PROPERTIES

        public bool IsEnabled { get; internal set; }

        public LoginRotatorItemViewState? CurrentItem { get; internal set; }

        #endregion
    }
}
