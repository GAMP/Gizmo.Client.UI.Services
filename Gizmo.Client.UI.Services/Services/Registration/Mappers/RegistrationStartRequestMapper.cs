namespace Gizmo.Client.UI.Services;

internal static class RegistrationStartRequestMapper
{
    public static Gizmo.Web.Api.Models.RegistrationStartModel Map(RegistrationStartRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new Gizmo.Web.Api.Models.RegistrationStartModel
        {
            IntegrationPublicId = request.IntegrationPublicId,
            DeliveryMethod      = RegistrationDeliveryMethodMapper.MapOutbound(request.DeliveryMethod),
            Email               = request.Email,
            PhoneNumber         = request.Phone
        };
    }
}
