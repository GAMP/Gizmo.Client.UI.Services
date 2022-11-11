using Gizmo.UI.View.States;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Base view state exposing language related state.
    /// </summary>
    public abstract class LanguagesViewStateBase : ViewStateBase
    {
        #region FIELDS
        private readonly List<RegionViewState> _regions = new();
        private RegionViewState? _selectedRegion;
        private readonly List<LanguageViewState> _languages = new();
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets available regions.
        /// </summary>
        public List<RegionViewState> Regions
        {
            get { return _regions; }
        }

        /// <summary>
        /// Gets selected language.
        /// </summary>
        public RegionViewState? SelectedRegion
        {
            get { return _selectedRegion; }
            internal set { SetProperty(ref _selectedRegion, value); }
        }

        /// <summary>
        /// Gets available languages.
        /// </summary>
        public List<LanguageViewState> Languages
        {
            get { return _languages; }
        }

        #endregion
    }
}