using Gizmo.UI.View.States;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Base view state exposing language related state.
    /// </summary>
    public abstract class LanguagesViewStateBase : ViewStateBase
    {
        #region PROPERTIES

        /// <summary>
        /// Gets available languages.
        /// </summary>
        public IEnumerable<LanguageViewState> Languages { get; internal set; } = Enumerable.Empty<LanguageViewState>();

        /// <summary>
        /// Gets selected language.
        /// </summary>
        public LanguageViewState SelectedLanguage { get; internal set; } = null!;

        #endregion
    }
}
