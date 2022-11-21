using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserAgreementViewState : ViewStateBase
    {
        #region FIELDS
        private int _id;
        private string _name = string.Empty;
        private string _text = string.Empty;
        private bool _isRejectable;
        private bool _ignoreState;
        private UserAgreementAcceptState _acceptState;
        #endregion

        #region PROPERTIES

        public int Id
        {
            get { return _id; }
            internal set { SetProperty(ref _id, value); }        
        }

        public string Name
        {
            get { return _name; }
            internal set { SetProperty(ref _name, value); }
        }

        public string Agreement
        {
            get { return _text; }
            internal set { SetProperty(ref _text, value); }
        }

        public bool IsRejectable
        {
            get { return _isRejectable; }
            internal set { SetProperty(ref _isRejectable, value); }
        }

        public bool IgnoreState
        {
            get { return _ignoreState; }
            internal set { SetProperty(ref _ignoreState, value); }
        }

        public UserAgreementAcceptState AcceptState
        {
            get { return _acceptState; }
            internal set { SetProperty(ref _acceptState, value); }
        }

        #endregion
    }
}
