namespace Gizmo.Client.UI.Services;

public sealed class PhoneValidationResult
{
    public bool    IsValid  { get; init; }
    public string? E164     { get; init; }
    public string? ErrorKey { get; init; }
}
