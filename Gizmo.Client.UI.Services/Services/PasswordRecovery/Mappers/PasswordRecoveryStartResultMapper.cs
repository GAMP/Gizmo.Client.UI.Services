using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

internal static class PasswordRecoveryStartResultMapper
{
    internal static PasswordRecoveryStartResult Map(VerificationStartResultModelBase model, string matchValue, PasswordRecoveryChannel channel)
    {
        ArgumentNullException.ThrowIfNull(model);

        return model switch
        {
            CodeInputRequiredResult codeInputRequired => new PasswordRecoveryStartResult.CodeInputRequired(
                Token:            codeInputRequired.Token,
                Destination:      ComputeDestination(matchValue, channel),
                CodeLength:       codeInputRequired.CodeLength,
                ExpiresInSeconds: codeInputRequired.ExpiresInSeconds),

            VerificationStartFailedResult failed => new PasswordRecoveryStartResult.Failed(PasswordRecoveryStartCodeMapper.Map(failed.Result)),

            RedirectRequiredResult => new PasswordRecoveryStartResult.Failed(PasswordRecoveryStartCode.Unknown),

            CallRequiredResult => new PasswordRecoveryStartResult.Failed(PasswordRecoveryStartCode.Unknown),

            _ => new PasswordRecoveryStartResult.Failed(PasswordRecoveryStartCode.Unknown),
        };
    }

    private static string ComputeDestination(string matchValue, PasswordRecoveryChannel channel) =>
        channel == PasswordRecoveryChannel.Email ? ContactMasking.MaskEmail(matchValue) : ContactMasking.MaskPhone(matchValue);
}
