using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ApplicationsPageViewState : ViewStateBase
    {
        #region PROPERTIES

        public IEnumerable<AppViewState> Applications { get; internal set; } = Enumerable.Empty<AppViewState>();


        /// <summary>
        /// Gets currently selected application category id.
        /// </summary>
        public int? SelectedCategoryId { get; internal set; }

        /// <summary>
        /// Gets current app categories.
        /// </summary>
        public IEnumerable<AppCategoryViewState> AppCategories { get; internal set; } = Enumerable.Empty<AppCategoryViewState>();

        #endregion
    }
}
