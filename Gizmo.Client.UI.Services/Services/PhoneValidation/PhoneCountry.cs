namespace Gizmo.Client.UI.Services;

public sealed class PhoneCountry
{
    public string RegionCode  { get; init; } = string.Empty;
    public string CountryName { get; init; } = string.Empty;
    public string CallingCode { get; init; } = string.Empty;
    public string? InputMask  { get; init; }
    public string? Placeholder { get; init; }
    public int MaxLength      { get; init; }
    public string? Flag       { get; init; }
}
