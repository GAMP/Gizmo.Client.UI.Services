using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserAgreementViewState : ViewStateBase
    {
        #region PROPERTIES

        public int Id { get; internal set; }

        public string? Name { get; internal set; }

        public string? Agreement { get; internal set; }

        public bool IsRejectable { get; internal set; }

        public bool IgnoreState { get; internal set; }

        public UserAgreementAcceptState AcceptState { get; internal set; }

        #endregion
    }
}
