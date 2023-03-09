using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AppExePersonalFileViewState : ViewStateBase
    {
        #region PROPERTIES

        public int PersonalFileId { get; internal set; }

        #endregion
    }
}
