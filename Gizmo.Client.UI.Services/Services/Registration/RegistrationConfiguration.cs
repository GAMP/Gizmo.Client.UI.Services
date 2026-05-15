namespace Gizmo.Client.UI.Services;

[Obsolete("Replaced by IReadOnlyList<RegistrationProvider> from GetProvidersAsync.")]
public sealed record RegistrationConfiguration(
    RegistrationVerificationMethod VerificationMethod,
    IReadOnlyList<RegistrationProvider> Providers
);
