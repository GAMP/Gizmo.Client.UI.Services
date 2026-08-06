using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services;

/// <remarks>
/// Shared between <see cref="PasswordRecoveryDestinationViewService"/> and
/// <see cref="PasswordRecoveryViewService"/> — both now start recovery in place and need the
/// same classification of the result into a session update plus a screen-local outcome.
/// </remarks>
internal static class PasswordRecoveryStartOutcome
{
    internal abstract record Result
    {
        private Result() { }

        internal sealed record Started : Result;
        internal sealed record Failed(string Message) : Result;
    }

    /// <remarks>
    /// The looked up value and its kind are already in the session by the time a start is issued —
    /// the input screen stores them before discovery branches — so this only records the start
    /// result itself.
    /// </remarks>
    internal static Result Apply(
        PasswordRecoveryStartResult startResult,
        IPasswordRecoverySessionService session,
        ILocalizationService localizationService)
    {
        switch (startResult)
        {
            case PasswordRecoveryStartResult.CodeInputRequired r:
                session.SetStartResult(r.Token, r.Destination ?? string.Empty, r.CodeLength, r.ExpiresInSeconds);
                return new Result.Started();

            case PasswordRecoveryStartResult.RedirectRequired r:
                session.SetRedirectStartResult(r.Token, r.RedirectUrl, r.ExpiresInSeconds);
                return new Result.Started();

            case PasswordRecoveryStartResult.CallRequired r:
                session.SetCallStartResult(r.Token, r.PhoneNumber, r.ExpiresInSeconds);
                return new Result.Started();

            case PasswordRecoveryStartResult.Failed { Code: PasswordRecoveryStartCode.NonUniqueInput }:
                return new Result.Failed(localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RECOVERY_NON_UNIQUE_INPUT)));

            case PasswordRecoveryStartResult.Failed { Code: PasswordRecoveryStartCode.UserNotFound or PasswordRecoveryStartCode.InvalidUserId }:
                return new Result.Failed(localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RECOVERY_USER_NOT_FOUND)));

            case PasswordRecoveryStartResult.Failed { Code: PasswordRecoveryStartCode.InvalidInput }:
                return new Result.Failed(localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_INVALID_FIELD)));

            default:
                return new Result.Failed(localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED)));
        }
    }
}
