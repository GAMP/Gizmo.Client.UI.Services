using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Sign up configuration view state service.
    /// </summary>
    [Register()]
    public sealed class UserRegistrationConfigurationViewState : ViewStateBase
    {
        #region FIELDS
        private bool _isEnabled;
        #endregion

        #region PROPERTIES   

        public bool IsEnabled
        {
            get { return _isEnabled; }
            internal set { _isEnabled = value; }
        }

        #endregion
    }
}
