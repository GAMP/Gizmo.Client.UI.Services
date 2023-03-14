using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class GlobalSearchResultViewState : ViewStateBase
    {
        #region PROPERTIES

        public SearchResultTypes Type { get; internal set; }

        public int Id { get; internal set; }

        public int CategoryId { get; internal set; }

        public string Name { get; internal set; } = null!;

        public int? ImageId { get; internal set; }

        #endregion
    }
}
