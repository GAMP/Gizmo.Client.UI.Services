using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

internal static class RegistrationStartResultMapper
{
    public static RegistrationStartResult Map(VerificationStartResultModelBase model,
                                              RegistrationStartRequest request)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(request);

        return model switch
        {
            CodeInputRequiredResult codeInputRequired => new RegistrationStartResult.CodeInputRequired(
                Token:            codeInputRequired.Token,
                Destination:      codeInputRequired.MaskedRecipientAddress ?? ComputeDestination(request),
                CodeLength:       codeInputRequired.CodeLength,
                ExpiresInSeconds: codeInputRequired.ExpiresInSeconds,
                CapabilityGuid:   codeInputRequired.CapabilityGuid),

            RedirectRequiredResult redirectRequired => new RegistrationStartResult.RedirectRequired(
                Token:            redirectRequired.Token,
                RedirectUrl:      redirectRequired.RedirectUrl,
                ExpiresInSeconds: redirectRequired.ExpiresInSeconds,
                CapabilityGuid:   redirectRequired.CapabilityGuid),

            VerificationStartFailedResult failed => new RegistrationStartResult.Failed(RegistrationStartCodeMapper.Map(failed.Result)),

            CallRequiredResult callRequired => new RegistrationStartResult.CallRequired(
                Token:            callRequired.Token,
                PhoneNumber:      callRequired.PhoneNumber,
                ExpiresInSeconds: callRequired.ExpiresInSeconds,
                CapabilityGuid:   callRequired.CapabilityGuid),

            _ => new RegistrationStartResult.Failed(RegistrationStartCode.Unknown),
        };
    }

    private static string? ComputeDestination(RegistrationStartRequest request) =>
        request.Kind switch
        {
            RegistrationStartKind.Email when request.Value != null       => ContactMasking.MaskEmail(request.Value),
            RegistrationStartKind.MobilePhone when request.Value != null => ContactMasking.MaskPhone(request.Value),
            _                                                           => null
        };
}
