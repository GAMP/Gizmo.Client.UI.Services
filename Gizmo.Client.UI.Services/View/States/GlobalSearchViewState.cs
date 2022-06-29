using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class GlobalSearchViewState : ViewStateBase
    {
        #region FIELDS
        private readonly HashSet<GlobalSearchResultViewState> _results = new();
        #endregion

        #region PROPERTIES
        
        /// <summary>
        /// Gets search results.
        /// </summary>
        public IEnumerable<GlobalSearchResultViewState> Results
        {
            get { return _results; }
        } 

        #endregion
    }
}
