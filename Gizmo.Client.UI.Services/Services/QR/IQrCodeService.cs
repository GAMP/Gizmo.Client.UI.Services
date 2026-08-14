namespace Gizmo.Client.UI.Services;

public interface IQrCodeService
{
    /// <summary>
    /// Generates QR code SVG from a URL string.
    /// Returns null on error.
    /// </summary>
    string? GenerateFromUrl(string url);
}
