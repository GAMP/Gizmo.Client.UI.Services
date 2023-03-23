using System.Diagnostics.CodeAnalysis;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// Equality comparer used to distinct between modification args.
    /// </summary>
    public sealed class ModificationEventArgsComparer : IEqualityComparer<ModificationEventArgs>
    {
        public static readonly ModificationEventArgsComparer Instance = new();

        public bool Equals(ModificationEventArgs? x, ModificationEventArgs? y)
        {
            if (x == null && y == null)
                return true;

            if ((x == null && y != null) || (x != null && y == null))
                return false;

            if (x?.EntityId == y?.EntityId && x?.ModificationType == y?.ModificationType) return true;

            return false;

        }

        public int GetHashCode([DisallowNull] ModificationEventArgs obj)
        {
            return obj.GetHashCode();
        }
    }
}
