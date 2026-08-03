using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services
{
    internal static class RegistrationProviderMapper
    {
        internal static RegistrationProvider Map(AvailableVerificationMethodModel source) =>
            new()
            {
                MethodId       = source.MethodId,
                Name           = source.Name,
                ChannelGuid    = source.ChannelGuid,
                CapabilityGuid = source.CapabilityGuid,
                HasChannel     = source.HasChannel,
                IsPrimary      = source.IsPrimary,
            };
    }
}
