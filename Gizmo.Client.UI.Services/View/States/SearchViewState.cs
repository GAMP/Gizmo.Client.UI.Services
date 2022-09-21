using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class SearchViewState : ViewStateBase
    {
        #region FIELDS
        private bool _isLoading;
        private bool _showAll;
        private bool _showAllLocally;
        private string _searchPattern = string.Empty;
        private List<SearchResultViewState> _applicationResults = new();
        private List<SearchResultViewState> _productResults = new();
        #endregion

        #region PROPERTIES

        public bool IsLoading
        {
            get { return _isLoading; }
            internal set { _isLoading = value; }
        }

        public bool ShowAll
        {
            get { return _showAll; }
            internal set { _showAll = value; }
        }

        public bool ShowAllLocally
        {
            get { return _showAllLocally; }
            internal set { _showAllLocally = value; }
        }

        public string SearchPattern
        {
            get { return _searchPattern; }
            internal set { _searchPattern = value; }
        }

        public List<SearchResultViewState> ApplicationResults
        {
            get { return _applicationResults; }
            internal set { _applicationResults = value; }
        }

        public List<SearchResultViewState> ProductResults
        {
            get { return _productResults; }
            internal set { _productResults = value; }
        }

        #endregion
    }
}