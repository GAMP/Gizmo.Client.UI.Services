namespace Gizmo.Client.UI.Services;

internal static class RegistrationDeliveryMethodMapper
{
    internal static RegistrationDeliveryMethod MapInbound(Gizmo.Web.Api.Models.VerificationDeliveryMethod source) =>
        source switch
        {
            Gizmo.Web.Api.Models.VerificationDeliveryMethod.Redirect     => RegistrationDeliveryMethod.Redirect,
            Gizmo.Web.Api.Models.VerificationDeliveryMethod.CodeDispatch => RegistrationDeliveryMethod.CodeDispatch,
            _                                                             => RegistrationDeliveryMethod.CodeDispatch,
        };

    internal static Gizmo.Web.Api.Models.VerificationDeliveryMethod MapOutbound(RegistrationDeliveryMethod source) =>
        source switch
        {
            RegistrationDeliveryMethod.Redirect     => Gizmo.Web.Api.Models.VerificationDeliveryMethod.Redirect,
            RegistrationDeliveryMethod.CodeDispatch => Gizmo.Web.Api.Models.VerificationDeliveryMethod.CodeDispatch,
            _                                       => Gizmo.Web.Api.Models.VerificationDeliveryMethod.CodeDispatch,
        };
}
