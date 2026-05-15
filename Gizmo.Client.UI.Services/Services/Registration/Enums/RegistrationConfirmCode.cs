namespace Gizmo.Client.UI.Services;

public enum RegistrationConfirmCode
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
