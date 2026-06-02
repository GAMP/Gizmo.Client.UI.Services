namespace Gizmo.Client.UI.Services;

internal static class PasswordRecoveryConfirmCodeMapper
{
    internal static PasswordRecoveryConfirmCode Map(Gizmo.VerificationCompleteResultCode source) =>
        source switch
        {
            Gizmo.VerificationCompleteResultCode.Success                 => PasswordRecoveryConfirmCode.Success,
            Gizmo.VerificationCompleteResultCode.Failure                 => PasswordRecoveryConfirmCode.Failure,
            Gizmo.VerificationCompleteResultCode.InvalidToken            => PasswordRecoveryConfirmCode.InvalidToken,
            Gizmo.VerificationCompleteResultCode.ExpiredToken            => PasswordRecoveryConfirmCode.ExpiredToken,
            Gizmo.VerificationCompleteResultCode.UsedToken               => PasswordRecoveryConfirmCode.UsedToken,
            Gizmo.VerificationCompleteResultCode.InvalidConfirmationCode => PasswordRecoveryConfirmCode.InvalidConfirmationCode,
            Gizmo.VerificationCompleteResultCode.AlreadyVerified         => PasswordRecoveryConfirmCode.AlreadyVerified,
            _                                                             => PasswordRecoveryConfirmCode.Unknown,
        };
}
