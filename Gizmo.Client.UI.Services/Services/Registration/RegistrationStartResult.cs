namespace Gizmo.Client.UI.Services
{
    public sealed record RegistrationStartResult(
        RegistrationStartCode Result,
        string? Token,
        string? Destination,
        int CodeLength,
        RegistrationDeliveryMethod? DeliveryMethod,
        string? RedirectUrl,
        int ExpiresInSeconds
    );
}
