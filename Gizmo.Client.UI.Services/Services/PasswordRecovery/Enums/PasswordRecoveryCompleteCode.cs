namespace Gizmo.Client.UI.Services;

public enum PasswordRecoveryCompleteCode
{
    Success,
    Failure,
    InvalidToken,
    InvalidTokenInput,
    ExpiredToken,
    UsedToken,
    RevokedToken,
    InvalidTokenType,
    Unknown,
}
