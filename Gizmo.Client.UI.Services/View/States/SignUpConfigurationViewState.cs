using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Sign up configuration view state service.
    /// </summary>
    [Register()]
    public sealed class SignUpConfigurationViewState : ViewStateBase
    {
        #region FIELDS
        private bool _canSignUp;
        #endregion

        #region PROPERTIES   

        public bool CanSignUp
        {
            get { return _canSignUp; }
            internal set { _canSignUp = value; }
        }

        #endregion
    }
}
