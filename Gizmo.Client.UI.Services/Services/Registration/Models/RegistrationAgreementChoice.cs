using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services
{
    public sealed class RegistrationAgreementChoice
    {
        public int AgreementId { get; init; }
        public UserAgreementAcceptState AcceptState { get; init; }
    }
}
