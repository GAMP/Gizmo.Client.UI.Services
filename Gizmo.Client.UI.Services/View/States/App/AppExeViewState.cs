using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AppExeViewState : ViewStateBase
    {
        #region PROPERTIES

        public int ExecutableId { get; internal set; }

        public int ApplicationId { get; internal set; }

        public string Caption { get; internal set; } = null!;

        public string Description { get; internal set; } = null!;

        public int DisplayOrder { get; internal set; }

        public IEnumerable<int> PersonalFiles { get; internal set; } = Enumerable.Empty<int>();

        public int? ImageId { get; internal set; }

        #endregion
    }
}
