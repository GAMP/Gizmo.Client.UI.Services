using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AppExeExecutionViewState : ViewStateBase
    {
        #region PROPERTIES

        public ExecutableState State { get; internal set; }

        /// <summary>
        /// Gets app exe id.
        /// </summary>
        public int AppExeId { get; internal set; }

        /// <summary>
        /// Gets app id.
        /// </summary>
        public int AppId { get; internal set; }

        /// <summary>
        /// Indicates if we are ready to start app exe process.
        /// </summary>
        public bool IsReady { get; internal set; }

        /// <summary>
        /// Indicates if previous execution have failed.
        /// </summary>
        public bool IsFailed { get; internal set; }

        /// <summary>
        /// Determines if any processes tracked (running) for this executable.
        /// </summary>
        public bool IsRunning { get; internal set; }

        /// <summary>
        /// Determines if this executable is active, meaning that some for of initialization (deployment,personal files etc) is running.
        /// </summary>
        public bool IsActive { get; internal set; }

        /// <summary>
        /// Gets overall progress, this value only have meaning if <see cref="IsActive"/> is true and <see cref="IsIndeterminate"/> is equal to false.
        /// </summary>
        /// <remarks>This value is in the range of 0-100.</remarks>
        public decimal Progress { get; internal set; }

        /// <summary>
        /// Indicates if progress can be determined.
        /// </summary>
        /// <remarks>
        /// This property only have meaning if <see cref="IsActive"/> is true.
        /// </remarks>
        public bool IsIndeterminate { get; internal set; }

        #endregion
    }
}
