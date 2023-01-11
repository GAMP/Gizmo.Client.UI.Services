using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserQuickLaunchViewState : ViewStateBase
    {
        #region FIELDS
        private int _selectedTabIndex = 0;
        #endregion

        #region PROPERTIES

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            internal set { _selectedTabIndex = value; }
        }

        #endregion
    }
}
