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

        public string? SearchPattern { get; internal set; }
        
        /// <summary>
        /// Gets currently selected application category id.
        /// </summary>
        public int? SelectedCategoryId { get; internal set; }

        public IEnumerable<EnumFilterViewState<ApplicationSortingOption>> SortingOptions { get; internal set; } = Enumerable.Empty<EnumFilterViewState<ApplicationSortingOption>>();

        public ApplicationSortingOption SelectedSortingOption { get; internal set; }

        public IEnumerable<EnumFilterViewState<ApplicationModes>> ExecutableModes { get; internal set; } = Enumerable.Empty<EnumFilterViewState<ApplicationModes>>();

        public IEnumerable<ApplicationModes> SelectedExecutableModes { get; internal set; } = Enumerable.Empty<ApplicationModes>();

        public int TotalFilters { get; internal set; }

        public byte CardsColumsCount { get; internal set; } = 8;

        #endregion
    }
}
