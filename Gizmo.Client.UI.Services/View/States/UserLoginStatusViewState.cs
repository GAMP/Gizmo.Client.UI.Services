using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// User login status view state.
    /// </summary>
    [Register()]
    public sealed class UserLoginStatusViewState : ViewStateBase
    {
        #region FIELDS
        private bool _loggedIn = false;
        private string? _username;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets if user is currently logged in.
        /// </summary>
        public bool IsLoggedIn
        {
            get { return _loggedIn; }
            internal set { _loggedIn = value; }
        }

        /// <summary>
        /// Gets username.
        /// </summary>
        public string? Username
        {
            get { return _username; }
            internal set { _username = value; }
        }

        #endregion
    }
}
