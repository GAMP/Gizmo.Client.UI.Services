using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class AppsPageViewState : ViewStateBase
    {
        #region PROPERTIES

        /// <summary>
        /// Gets current app categories.
        /// </summary>
        public IEnumerable<AppCategoryViewState> AppCategories { get; internal set; } = Enumerable.Empty<AppCategoryViewState>();

        public IEnumerable<AppViewState> Applications { get; internal set; } = Enumerable.Empty<AppViewState>();

        public string SearchPattern { get; internal set; } = null!;
        
        /// <summary>
        /// Gets currently selected application category id.
        /// </summary>
        public int? SelectedCategoryId { get; internal set; }

        public IEnumerable<ApplicationFilterViewState> SortingOptions { get; internal set; } = Enumerable.Empty<ApplicationFilterViewState>();

        public int SelectedSortingOption { get; internal set; }

        public IEnumerable<ApplicationFilterViewState> ExecutableModes { get; internal set; } = Enumerable.Empty<ApplicationFilterViewState>();

        public IEnumerable<int> SelectedExecutableModes { get; internal set; } = Enumerable.Empty<int>();

        public int TotalFilters { get; internal set; }

        #endregion
    }
}
