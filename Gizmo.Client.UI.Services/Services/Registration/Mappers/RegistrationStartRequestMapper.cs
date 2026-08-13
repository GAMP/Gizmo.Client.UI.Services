namespace Gizmo.Client.UI.Services;

internal static class RegistrationStartRequestMapper
{
    public static Gizmo.Web.Api.Models.UserRegistrationMethodStartModel Map(RegistrationStartRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Gizmo.Web.Api.Models.UserRegistrationMethodStartModel
        {
            MethodId = request.MethodId,
            Value = request.Kind switch
            {
                RegistrationStartKind.MobilePhone or RegistrationStartKind.Email => request.Value,
                RegistrationStartKind.Redirect                                   => null,
                _ => throw new ArgumentOutOfRangeException(nameof(request.Kind), request.Kind, "Unsupported registration start kind.")
            }
        };
    }
}
