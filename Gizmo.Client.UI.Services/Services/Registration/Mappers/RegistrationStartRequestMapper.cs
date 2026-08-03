namespace Gizmo.Client.UI.Services;

internal static class RegistrationStartRequestMapper
{
    public static Gizmo.Web.Api.Models.RegistrationMethodStartModel Map(RegistrationStartRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return request.Kind switch
        {
            RegistrationStartKind.MobilePhone => new Gizmo.Web.Api.Models.RegistrationByMobilePhoneMethodStartModel
            {
                MethodId = request.MethodId,
                Value    = request.Value
            },
            RegistrationStartKind.Email => new Gizmo.Web.Api.Models.RegistrationByEmailMethodStartModel
            {
                MethodId = request.MethodId,
                Value    = request.Value
            },
            RegistrationStartKind.Redirect => new Gizmo.Web.Api.Models.RegistrationByRedirectMethodStartModel
            {
                MethodId = request.MethodId,
                Value    = request.Value
            },
            _ => throw new ArgumentOutOfRangeException(nameof(request.Kind), request.Kind, "Unsupported registration start kind.")
        };
    }
}
