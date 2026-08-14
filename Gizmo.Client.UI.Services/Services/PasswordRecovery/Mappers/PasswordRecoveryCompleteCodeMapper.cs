namespace Gizmo.Client.UI.Services;

internal static class PasswordRecoveryCompleteCodeMapper
{
    internal static PasswordRecoveryCompleteCode Map(Gizmo.PasswordRecoveryCompleteResultCode source) =>
        source switch
        {
            Gizmo.PasswordRecoveryCompleteResultCode.Success           => PasswordRecoveryCompleteCode.Success,
            Gizmo.PasswordRecoveryCompleteResultCode.Failure           => PasswordRecoveryCompleteCode.Failure,
            Gizmo.PasswordRecoveryCompleteResultCode.InvalidToken      => PasswordRecoveryCompleteCode.InvalidToken,
            Gizmo.PasswordRecoveryCompleteResultCode.InvalidTokenInput => PasswordRecoveryCompleteCode.InvalidTokenInput,
            Gizmo.PasswordRecoveryCompleteResultCode.ExpiredToken      => PasswordRecoveryCompleteCode.ExpiredToken,
            Gizmo.PasswordRecoveryCompleteResultCode.UsedToken         => PasswordRecoveryCompleteCode.UsedToken,
            Gizmo.PasswordRecoveryCompleteResultCode.RevokedToken      => PasswordRecoveryCompleteCode.RevokedToken,
            Gizmo.PasswordRecoveryCompleteResultCode.InvalidTokenType  => PasswordRecoveryCompleteCode.InvalidTokenType,
            _                                                          => PasswordRecoveryCompleteCode.Unknown,
        };
}
