using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

internal static class PasswordRecoveryStartResultMapper
{
    internal static PasswordRecoveryStartResult Map(VerificationStartResultModelBase model, PasswordRecoveryStartRequest request)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(request);

        return model switch
        {
            CodeInputRequiredResult codeInputRequired => new PasswordRecoveryStartResult.CodeInputRequired(
                Token:            codeInputRequired.Token,
                Destination:      codeInputRequired.MaskedRecipientAddress ?? ComputeDestination(request),
                CodeLength:       codeInputRequired.CodeLength,
                ExpiresInSeconds: codeInputRequired.ExpiresInSeconds,
                CapabilityGuid:   codeInputRequired.CapabilityGuid),

            RedirectRequiredResult redirectRequired => new PasswordRecoveryStartResult.RedirectRequired(
                Token:            redirectRequired.Token,
                RedirectUrl:      redirectRequired.RedirectUrl,
                ExpiresInSeconds: redirectRequired.ExpiresInSeconds,
                CapabilityGuid:   redirectRequired.CapabilityGuid),

            CallRequiredResult callRequired => new PasswordRecoveryStartResult.CallRequired(
                Token:            callRequired.Token,
                PhoneNumber:      callRequired.PhoneNumber,
                ExpiresInSeconds: callRequired.ExpiresInSeconds,
                CapabilityGuid:   callRequired.CapabilityGuid),

            VerificationStartFailedResult failed => new PasswordRecoveryStartResult.Failed(PasswordRecoveryStartCodeMapper.Map(failed.Result)),

            _ => new PasswordRecoveryStartResult.Failed(PasswordRecoveryStartCode.Unknown),
        };
    }

    private static string? ComputeDestination(PasswordRecoveryStartRequest request) =>
        request.IdentifierKind switch
        {
            PasswordRecoveryIdentifierKind.Email       => ContactMasking.MaskEmail(request.Value),
            PasswordRecoveryIdentifierKind.MobilePhone => ContactMasking.MaskPhone(request.Value),
            _                                          => null
        };
}
