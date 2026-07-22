using Gizmo;
using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

internal static class PasswordRecoveryProviderMapper
{
    internal static PasswordRecoveryProvider Map(VerificationProviderModel source) =>
        new()
        {
            PublicId  = source.PublicId,
            Name      = source.Name ?? string.Empty,
            Channel   = ToChannel(source.ChannelGuid),
            IsPrimary = source.IsPrimary,
        };

    internal static PasswordRecoveryChannel? TryGetChannel(Guid channelGuid)
    {
        if (channelGuid == new Guid(CommunicationChannels.Email))
            return PasswordRecoveryChannel.Email;
        if (channelGuid == new Guid(CommunicationChannels.Sms))
            return PasswordRecoveryChannel.Sms;
        return null;
    }

    private static PasswordRecoveryChannel ToChannel(Guid channelGuid) =>
        TryGetChannel(channelGuid) ?? throw new InvalidOperationException($"Unsupported channel {channelGuid}.");
}
