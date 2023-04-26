using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class HostQRCodeViewState : ViewStateBase
    {
        public bool IsEnabled { get; internal set; }

        public string? HostQRCode { get; internal set; }
    }
}
