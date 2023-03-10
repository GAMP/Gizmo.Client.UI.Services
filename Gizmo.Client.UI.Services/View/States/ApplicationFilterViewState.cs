using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ApplicationFilterViewState : ViewStateBase
    {
        #region PROPERTIES

        public int Id { get; internal set; }

        public string Name { get; internal set; } = null!;

        #endregion
    }
}
