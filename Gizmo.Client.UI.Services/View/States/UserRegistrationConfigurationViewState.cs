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
        private bool _isDirectEnabled;
        private bool _isPasswordRecoveryEnabled;
        #endregion

        #region PROPERTIES   

        public bool IsEnabled
        {
            get { return _isEnabled; }
            internal set { _isEnabled = value; }
        }

        public bool IsDirectEnabled
        {
            get { return _isDirectEnabled; }
            internal set { _isDirectEnabled = value; }
        }

        public bool IsPasswordRecoveryEnabled
        {
            get { return _isPasswordRecoveryEnabled; }
            internal set { _isPasswordRecoveryEnabled = value; }
        }

        #endregion
    }
}
