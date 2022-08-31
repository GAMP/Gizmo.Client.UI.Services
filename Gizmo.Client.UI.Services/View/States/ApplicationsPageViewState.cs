using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ApplicationsPageViewState : ViewStateBase
    {
        #region FIELDS
        private List<ApplicationGroupViewState> _applicationGroups = new();
        private List<ApplicationViewState> _applications = new();
        #endregion

        #region PROPERTIES

        public List<ApplicationGroupViewState> ApplicationGroups
        {
            get { return _applicationGroups; }
            internal set { _applicationGroups = value; }
        }

        public List<ApplicationViewState> Applications
        {
            get { return _applications; }
            internal set { _applications = value; }
        }

        #endregion
    }
}
