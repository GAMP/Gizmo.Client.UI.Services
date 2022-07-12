using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserPasswordRecoveryMethodViewState : ViewStateBase
    {
        #region FIELDS
        private UserPasswordRecoveryMethod _method;
        #endregion

        #region PROPERTIES

        public UserPasswordRecoveryMethod Method
        {
            get { return _method; }
            internal set { SetProperty(ref _method, value); }
        } 
        
        #endregion
    }
}
