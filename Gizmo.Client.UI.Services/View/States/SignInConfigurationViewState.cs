using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Sign in configuration view state.
    /// </summary>
    [Register()]
    public sealed class SignInConfigurationViewState : ViewStateBase
    {
        private bool _canSignIn;
        private bool _canSignInWithQR;

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
    }
}
