using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class LoginWallpaperViewState : ViewStateBase
    {
        #region PROPERTIES

        public string Wallpaper { get; internal set; } = string.Empty;

        #endregion
    }
}
