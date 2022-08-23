using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class HomePageViewState : ViewStateBase
    {
        #region FIELDS
        private List<ApplicationViewState> _newApplications = new();
        private List<ApplicationViewState> _topRatedApplications = new();
        private List<ApplicationViewState> _mostUsedApplications = new();
        #endregion

        #region PROPERTIES

        public List<ApplicationViewState> NewApplications
        {
            get { return _newApplications; }
            internal set { _newApplications = value; }
        }

        public List<ApplicationViewState> TopRatedApplications
        {
            get { return _topRatedApplications; }
            internal set { _topRatedApplications = value; }
        }

        public List<ApplicationViewState> MostUsedApplications
        {
            get { return _mostUsedApplications; }
            internal set { _mostUsedApplications = value; }
        }

        #endregion
    }
}
