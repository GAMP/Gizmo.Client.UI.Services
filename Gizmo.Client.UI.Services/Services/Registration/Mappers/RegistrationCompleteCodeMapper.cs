namespace Gizmo.Client.UI.Services;

internal static class RegistrationCompleteCodeMapper
{
    internal static RegistrationCompleteCode Map(Gizmo.AccountCreationByTokenCompleteResultCode source) =>
        source switch
        {
            Gizmo.AccountCreationByTokenCompleteResultCode.Success          => RegistrationCompleteCode.Success,
            Gizmo.AccountCreationByTokenCompleteResultCode.Failure          => RegistrationCompleteCode.Failure,
            Gizmo.AccountCreationByTokenCompleteResultCode.InvalidToken     => RegistrationCompleteCode.InvalidToken,
            Gizmo.AccountCreationByTokenCompleteResultCode.InvalidTokenInput => RegistrationCompleteCode.InvalidToken,
            Gizmo.AccountCreationByTokenCompleteResultCode.ExpiredToken     => RegistrationCompleteCode.ExpiredToken,
            Gizmo.AccountCreationByTokenCompleteResultCode.UsedToken        => RegistrationCompleteCode.UsedToken,
            Gizmo.AccountCreationByTokenCompleteResultCode.InvalidInput     => RegistrationCompleteCode.InvalidInput,
            Gizmo.AccountCreationByTokenCompleteResultCode.NoUserGroup      => RegistrationCompleteCode.NoUserGroup,
            _                                                               => RegistrationCompleteCode.Unknown,
        };

    internal static RegistrationCompleteCode Map(Gizmo.AccountCreationCompleteResultCode source) =>
        source switch
        {
            Gizmo.AccountCreationCompleteResultCode.Success         => RegistrationCompleteCode.Success,
            Gizmo.AccountCreationCompleteResultCode.Failure         => RegistrationCompleteCode.Failure,
            Gizmo.AccountCreationCompleteResultCode.InvalidInput    => RegistrationCompleteCode.InvalidInput,
            Gizmo.AccountCreationCompleteResultCode.NoUserGroup     => RegistrationCompleteCode.NoUserGroup,
            Gizmo.AccountCreationCompleteResultCode.NonUniqueInput  => RegistrationCompleteCode.NonUniqueInput,
            _                                                       => RegistrationCompleteCode.Unknown,
        };
}
