namespace Gizmo.Client.UI.Services;

internal static class RegistrationCompleteRequestMapper
{
    public static Gizmo.Web.Api.Models.RegistrationCompleteModel MapComplete(RegistrationCompleteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new Gizmo.Web.Api.Models.RegistrationCompleteModel
        {
            Token    = request.Token,
            Password = request.Password,
            Profile  = RegistrationProfileMapper.Map(request.Profile)
        };
    }

    public static Gizmo.Web.Api.Models.RegistrationDirectModel MapDirect(RegistrationCompleteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new Gizmo.Web.Api.Models.RegistrationDirectModel
        {
            Password = request.Password,
            Profile  = RegistrationProfileMapper.Map(request.Profile)
        };
    }
}
