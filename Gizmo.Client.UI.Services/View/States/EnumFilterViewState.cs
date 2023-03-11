using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class EnumFilterViewState<T> : ViewStateBase
    {
        #region PROPERTIES

        public T Value { get; internal set; } = default!;

        public string DisplayName { get; internal set; } = null!;

        #endregion
    }
}
