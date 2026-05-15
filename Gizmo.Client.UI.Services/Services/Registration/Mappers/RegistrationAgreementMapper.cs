using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services
{
    internal static class RegistrationAgreementMapper
    {
        internal static RegistrationAgreement Map(UserAgreementModel source) =>
            new()
            {
                Id           = source.Id,
                Name         = source.Name,
                Agreement    = source.Agreement,
                IsRejectable = source.IsRejectable,
                IgnoreState  = source.IgnoreState,
            };
    }
}
