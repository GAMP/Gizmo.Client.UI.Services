namespace Gizmo.Client.UI.Services
{
    public sealed class RegistrationCompleteRequest
    {
        public string? Token { get; init; }
        public RegistrationProfile Profile { get; init; } = null!;
        public string? Password { get; init; }
    }
}
