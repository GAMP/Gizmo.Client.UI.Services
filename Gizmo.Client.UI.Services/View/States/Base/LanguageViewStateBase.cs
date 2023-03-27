using Gizmo.UI.View.States;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Base view state exposing language related state.
    /// </summary>
    public abstract class LanguagesViewStateBase : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<LanguageViewState> _languages = Enumerable.Empty<LanguageViewState>();
        private LanguageViewState? _selectedLanguage;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets available languages.
        /// </summary>
        public IEnumerable<LanguageViewState> Languages
        {
            get { return _languages; }
            internal set { _languages = value; }
        }

        /// <summary>
        /// Gets selected language.
        /// </summary>
        public LanguageViewState? SelectedLanguage
        {
            get { return _selectedLanguage; }
            internal set { _selectedLanguage = value; }
        }

        #endregion
    }
}
