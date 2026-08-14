namespace Gizmo.Client.UI.Services
{
    public sealed class RegistrationCompleteRequest
    {
        public string? Token { get; init; }
        public RegistrationProfile Profile { get; init; } = null!;
        public string? Password { get; init; }
        public IReadOnlyList<RegistrationAgreementChoice> AgreementStates { get; init; } = Array.Empty<RegistrationAgreementChoice>();
    }
}
