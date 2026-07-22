using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services
{
    internal static class RegistrationProviderMapper
    {
        internal static RegistrationProvider Map(VerificationProviderModel source) =>
            new()
            {
                PublicId        = source.PublicId,
                Name            = source.Name,
                ChannelGuid     = source.ChannelGuid,
                CanRedirect     = source.CanRedirect,
                CanDispatchCode = source.CanDispatchCode,
                CanProvideEmail = source.CanProvideEmail,
                CanProvidePhone = source.CanProvidePhone,
                HasChannel      = source.HasChannel,
                IsPrimary       = source.IsPrimary,
            };
    }
}
