namespace Gizmo.Client.UI.Services;

internal static class RegistrationConfirmCodeMapper
{
    internal static RegistrationConfirmCode Map(Gizmo.VerificationCompleteResultCode source) =>
        source switch
        {
            Gizmo.VerificationCompleteResultCode.Success                => RegistrationConfirmCode.Success,
            Gizmo.VerificationCompleteResultCode.Failure                => RegistrationConfirmCode.Failure,
            Gizmo.VerificationCompleteResultCode.InvalidToken           => RegistrationConfirmCode.InvalidToken,
            Gizmo.VerificationCompleteResultCode.ExpiredToken           => RegistrationConfirmCode.ExpiredToken,
            Gizmo.VerificationCompleteResultCode.UsedToken              => RegistrationConfirmCode.UsedToken,
            Gizmo.VerificationCompleteResultCode.InvalidConfirmationCode => RegistrationConfirmCode.InvalidConfirmationCode,
            Gizmo.VerificationCompleteResultCode.AlreadyVerified        => RegistrationConfirmCode.AlreadyVerified,
            _                                                            => RegistrationConfirmCode.Unknown,
        };
}
