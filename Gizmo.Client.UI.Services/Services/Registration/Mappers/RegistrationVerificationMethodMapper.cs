namespace Gizmo.Client.UI.Services;

[Obsolete("Obsolete together with RegistrationVerificationMethod.")]
internal static class RegistrationVerificationMethodMapper
{
    internal static RegistrationVerificationMethod Map(Gizmo.Server.RegistrationVerificationMethod source) =>
        source switch
        {
            Gizmo.Server.RegistrationVerificationMethod.Email       => RegistrationVerificationMethod.Email,
            Gizmo.Server.RegistrationVerificationMethod.MobilePhone => RegistrationVerificationMethod.MobilePhone,
            _                                                        => RegistrationVerificationMethod.None,
        };
}
