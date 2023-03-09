using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class PersonalFileViewState : ViewStateBase
    {
        #region PROPERTIES

        public int PersonalFileId { get; internal set; }

        public string Caption { get; internal set; } = null!;

        #endregion
    }
}
