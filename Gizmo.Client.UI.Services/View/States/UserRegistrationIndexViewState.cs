using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationIndexViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        public IEnumerable<UserAgreementModelState> UserAgreementStates { get; internal set; } = Enumerable.Empty<UserAgreementModelState>();

        #endregion
    }
}
