namespace Gizmo.Client.UI.Services;

[Obsolete("Replaced by provider ChannelGuid-based routing. Remove when separate pages per method implemented.")]
public enum RegistrationVerificationMethod
{
    None = 0,
    Email = 1,
    MobilePhone = 2,
}
