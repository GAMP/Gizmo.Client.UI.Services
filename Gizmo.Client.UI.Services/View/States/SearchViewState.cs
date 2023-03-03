using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class SearchViewState : ViewStateBase
    {
        #region FIELDS
        private bool _openDropDown;
        private bool _isLoading;
        private bool _showAll;
        
        private string _searchPattern = string.Empty;
        private IEnumerable<SearchResultViewState> _applicationResults = Enumerable.Empty<SearchResultViewState>();
        private IEnumerable<SearchResultViewState> _productResults = Enumerable.Empty<SearchResultViewState>();

        private string _appliedSearchPattern = string.Empty;
        private IEnumerable<SearchResultViewState> _appliedApplicationResults = Enumerable.Empty<SearchResultViewState>();
        private IEnumerable<SearchResultViewState> _appliedProductResults = Enumerable.Empty<SearchResultViewState>();
        #endregion

        #region PROPERTIES

        public bool OpenDropDown
        {
            get { return _openDropDown; }
            internal set { _openDropDown = value; }
        }

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

        public string SearchPattern
        {
            get { return _searchPattern; }
            internal set { _searchPattern = value; }
        }

        public IEnumerable<SearchResultViewState> ApplicationResults
        {
            get { return _applicationResults; }
            internal set { _applicationResults = value; }
        }

        public IEnumerable<SearchResultViewState> ProductResults
        {
            get { return _productResults; }
            internal set { _productResults = value; }
        }

        public string AppliedSearchPattern
        {
            get { return _appliedSearchPattern; }
            internal set { _appliedSearchPattern = value; }
        }

        public IEnumerable<SearchResultViewState> AppliedApplicationResults
        {
            get { return _appliedApplicationResults; }
            internal set { _appliedApplicationResults = value; }
        }

        public IEnumerable<SearchResultViewState> AppliedProductResults
        {
            get { return _appliedProductResults; }
            internal set { _appliedProductResults = value; }
        }

        #endregion
    }
}
