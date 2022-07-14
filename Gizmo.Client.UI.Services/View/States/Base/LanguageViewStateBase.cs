using Gizmo.UI.View.States;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Base view state exposing language related state.
    /// </summary>
    public abstract class LanguageViewStateBase : ViewStateBase
    {
        #region FIELDS
        private LanguageViewState? _selectedLanguage;
        private readonly List<LanguageViewState> _languages = new();
        private readonly List<RegionViewState> _regions = new();
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets selected language.
        /// </summary>
        public LanguageViewState? SelectedLanguage
        {
            get { return _selectedLanguage; }
            internal set { SetProperty(ref _selectedLanguage, value); }
        }

        /// <summary>
        /// Gets available languages.
        /// </summary>
        public List<LanguageViewState> Languages
        {
            get { return _languages; }
        } 

        /// <summary>
        /// Gets available regions.
        /// </summary>
        public List<RegionViewState> Regions
        {
            get { return _regions; }
        }
        
        #endregion
    }
}
