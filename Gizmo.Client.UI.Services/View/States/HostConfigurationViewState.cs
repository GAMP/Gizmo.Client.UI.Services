using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class HostConfigurationViewState : ViewStateBase
    {
        #region FIELDS
        private bool _canSignIn;
        private bool _canSignInWithQR;
        private bool _canSignUp;
        private bool _canRecoverPassword;
        #endregion

        #region PROPERTIES

        public bool CanSignIn
        {
            get { return _canSignIn; }
            internal set { _canSignIn = value; }
        }

        public bool CanSignInWithQR
        {
            get { return _canSignInWithQR; }
            internal set { _canSignInWithQR = value; }
        }

        public bool CanSignUp
        {
            get { return _canSignUp; }
            internal set { _canSignUp = value; }
        }

        public bool CanRecoverPassword
        {
            get { return _canRecoverPassword; }
            internal set { _canRecoverPassword = value; }
        }

        #endregion
    }
}