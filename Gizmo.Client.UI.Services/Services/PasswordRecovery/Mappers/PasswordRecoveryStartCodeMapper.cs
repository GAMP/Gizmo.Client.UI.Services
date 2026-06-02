namespace Gizmo.Client.UI.Services;

internal static class PasswordRecoveryStartCodeMapper
{
    internal static PasswordRecoveryStartCode Map(Gizmo.VerificationStartResultCode source) =>
        source switch
        {
            Gizmo.VerificationStartResultCode.Success            => PasswordRecoveryStartCode.Success,
            Gizmo.VerificationStartResultCode.Failed             => PasswordRecoveryStartCode.Failed,
            Gizmo.VerificationStartResultCode.NoRouteForDelivery => PasswordRecoveryStartCode.NoRouteForDelivery,
            Gizmo.VerificationStartResultCode.DeliveryFailed     => PasswordRecoveryStartCode.DeliveryFailed,
            Gizmo.VerificationStartResultCode.InvalidInput       => PasswordRecoveryStartCode.InvalidInput,
            Gizmo.VerificationStartResultCode.NonUniqueInput     => PasswordRecoveryStartCode.NonUniqueInput,
            _                                                     => PasswordRecoveryStartCode.Unknown,
        };
}
