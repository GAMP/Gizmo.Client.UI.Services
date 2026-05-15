namespace Gizmo.Client.UI.Services;

public enum RegistrationCompleteCode
{
    Success,
    Failure,
    InvalidToken,
    ExpiredToken,
    UsedToken,
    InvalidInput,
    NoUserGroup,
    NonUniqueInput,
    Unknown
}
