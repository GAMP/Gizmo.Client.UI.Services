namespace Gizmo.Client.UI.Services;

public enum PasswordRecoveryConfirmCode
{
    Success,
    Failure,
    InvalidToken,
    ExpiredToken,
    UsedToken,
    InvalidConfirmationCode,
    AlreadyVerified,
    Unknown,
}
