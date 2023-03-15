using System.Globalization;
using Gizmo.Client.Interfaces;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Default culture service implementaion.
    /// </summary>
    /// <remarks>
    /// This implementation should be used in hosts that dont need any extra culture handling for setting culture globally such as single thread blazor web assembly web hosts.
    /// </remarks>
    public sealed class CultureService : ICultureService
    {
        public Task SetCurrentUICultureAsync(CultureInfo culture)
        {
            CultureInfo.CurrentUICulture = culture;
            return Task.CompletedTask;  
        }
    }
}
