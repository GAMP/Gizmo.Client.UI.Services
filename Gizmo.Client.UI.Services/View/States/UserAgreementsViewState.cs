using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserAgreementsViewState : ViewStateBase
    {
        #region FIELDS
        private int? _userId;
        private List<UserAgreementViewState> _userAgreements = new();
        private int? _currentUserAgreementIndex;
        private UserAgreementViewState? _currentUserAgreement;
        #endregion

        #region PROPERTIES

        public int? UserId
        {
            get { return _userId; }
            internal set { _userId = value; }
        }

        public List<UserAgreementViewState> UserAgreements
        {
            get { return _userAgreements; }
            internal set { _userAgreements = value; }
        }

        public bool HasUserAgreements
        {
            get { return CurrentUserAgreement != null; }
        }

        public int? CurrentUserAgreementIndex
        {
            get { return _currentUserAgreementIndex; }
            internal set { _currentUserAgreementIndex = value; }
        }

        public UserAgreementViewState? CurrentUserAgreement
        {
            get { return _currentUserAgreement; }
            internal set { _currentUserAgreement = value; }
        }

        #endregion
    }
}
