namespace Gizmo.Client.UI.Services;

public sealed class RegistrationStartRequest
{
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public RegistrationDeliveryMethod DeliveryMethod { get; init; }
    public Guid IntegrationPublicId { get; init; }
}
