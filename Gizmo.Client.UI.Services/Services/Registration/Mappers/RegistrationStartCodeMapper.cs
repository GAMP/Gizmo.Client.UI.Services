namespace Gizmo.Client.UI.Services;

internal static class RegistrationStartCodeMapper
{
    internal static RegistrationStartCode Map(Gizmo.VerificationStartResultCode source) =>
        source switch
        {
            Gizmo.VerificationStartResultCode.Success            => RegistrationStartCode.Success,
            Gizmo.VerificationStartResultCode.Failed             => RegistrationStartCode.Failed,
            Gizmo.VerificationStartResultCode.NoRouteForDelivery => RegistrationStartCode.NoRouteForDelivery,
            Gizmo.VerificationStartResultCode.DeliveryFailed     => RegistrationStartCode.DeliveryFailed,
            Gizmo.VerificationStartResultCode.InvalidInput       => RegistrationStartCode.InvalidInput,
            Gizmo.VerificationStartResultCode.NonUniqueInput     => RegistrationStartCode.NonUniqueInput,
            _                                                     => RegistrationStartCode.Unknown,
        };
}
