using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AppViewState : ViewStateBase
    {
        #region PROPERTIES

        public int ApplicationId { get; internal set; }

        public string Title { get; internal set; } = null!;

        public string Description { get; internal set; } = null!;

        public int ApplicationCategoryId { get; internal set; }

        public DateTime? ReleaseDate { get; internal set; }

        public DateTime AddDate { get; internal set; }

        public int? DeveloperId { get; internal set; }

        public int? PublisherId { get; internal set; }

        public int? ImageId { get; internal set; }

        #endregion
    }
}
