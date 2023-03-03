﻿using Gizmo.UI.View.States;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Base view state exposing language related state.
    /// </summary>
    public abstract class LanguagesViewStateBase : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<RegionViewState> _regions = Enumerable.Empty<RegionViewState>();
        private RegionViewState? _selectedRegion;
        private IEnumerable<LanguageViewState> _languages = Enumerable.Empty<LanguageViewState>();
        private LanguageViewState? _selectedLanguage;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets available regions.
        /// </summary>
        public IEnumerable<RegionViewState> Regions
        {
            get { return _regions; }
            internal set { SetProperty(ref _regions, value); }
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
        public IEnumerable<LanguageViewState> Languages
        {
            get { return _languages; }
            internal set { SetProperty(ref _languages, value); }
        }

        /// <summary>
        /// Gets selected language.
        /// </summary>
        public LanguageViewState? SelectedLanguage
        {
            get { return _selectedLanguage; }
            internal set { SetProperty(ref _selectedLanguage, value); }
        }

        #endregion
    }
}
