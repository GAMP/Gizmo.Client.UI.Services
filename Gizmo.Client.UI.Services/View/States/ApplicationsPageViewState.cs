using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ApplicationsPageViewState : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<ApplicationGroupViewState> _applicationGroups = Enumerable.Empty<ApplicationGroupViewState>();
        private IEnumerable<ApplicationViewState> _applications = Enumerable.Empty<ApplicationViewState>();
        #endregion

        #region PROPERTIES

        public IEnumerable<ApplicationGroupViewState> ApplicationGroups
        {
            get { return _applicationGroups; }
            internal set { _applicationGroups = value; }
        }

        public IEnumerable<ApplicationViewState> Applications
        {
            get { return _applications; }
            internal set { _applications = value; }
        }

        #endregion
    }
}
