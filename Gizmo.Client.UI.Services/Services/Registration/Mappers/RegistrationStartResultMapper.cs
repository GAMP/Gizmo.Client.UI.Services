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
                Destination:      ComputeDestination(request),
                CodeLength:       codeInputRequired.CodeLength,
                ExpiresInSeconds: codeInputRequired.ExpiresInSeconds),

            RedirectRequiredResult redirectRequired => new RegistrationStartResult.RedirectRequired(
                Token:            redirectRequired.Token,
                RedirectUrl:      redirectRequired.RedirectUrl,
                ExpiresInSeconds: redirectRequired.ExpiresInSeconds),

            VerificationStartFailedResult failed => new RegistrationStartResult.Failed(RegistrationStartCodeMapper.Map(failed.Result)),

            CallRequiredResult => new RegistrationStartResult.Failed(RegistrationStartCode.Unknown),

            _ => new RegistrationStartResult.Failed(RegistrationStartCode.Unknown),
        };
    }

    private static string? ComputeDestination(RegistrationStartRequest request) =>
        request.DeliveryMethod switch
        {
            _ when request.Email != null => ContactMasking.MaskEmail(request.Email),
            _ when request.Phone != null => ContactMasking.MaskPhone(request.Phone),
            _                             => null
        };
}
