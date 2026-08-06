namespace Gizmo.Client.UI.Services;

internal static class RegistrationStartRequestMapper
{
    public static Gizmo.Web.Api.Models.VerificationMethodStartModelBase Map(RegistrationStartRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Gizmo.Web.Api.Models.VerificationMethodStartModelBase
        {
            MethodId  = request.MethodId,
            Value     = request.Kind == RegistrationStartKind.Redirect ? null : request.Value,
            ValueKind = request.Kind switch
            {
                RegistrationStartKind.MobilePhone => Gizmo.Web.Api.Models.VerificationMethodValueKind.MobilePhone,
                RegistrationStartKind.Email       => Gizmo.Web.Api.Models.VerificationMethodValueKind.Email,
                RegistrationStartKind.Redirect     => Gizmo.Web.Api.Models.VerificationMethodValueKind.None,
                _ => throw new ArgumentOutOfRangeException(nameof(request.Kind), request.Kind, "Unsupported registration start kind.")
            }
        };
    }
}
