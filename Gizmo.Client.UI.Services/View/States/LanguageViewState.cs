using Gizmo.UI.View.States;

using Microsoft.Extensions.DependencyInjection;

using System.Globalization;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class LanguageViewState : ViewStateBase
    {
        public CultureInfo Culture { get; internal set; } = CultureInfo.CurrentCulture;
    }
}
