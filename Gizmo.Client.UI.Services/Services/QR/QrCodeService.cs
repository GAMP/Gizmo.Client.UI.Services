using Net.Codecrete.QrCodeGenerator;

namespace Gizmo.Client.UI.Services;

public sealed class QrCodeService : IQrCodeService
{
    public string? GenerateFromUrl(string url)
    {
        try
        {
            var qr = QrCode.EncodeText(url, QrCode.Ecc.Medium);
            return qr.ToSvgString(border: 4);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
