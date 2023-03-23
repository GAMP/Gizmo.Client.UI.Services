using Gizmo.UI.View.Services;

namespace Gizmo.Client.UI.Services
{
    public static class EnumExtensions
    {
        public static LookupServiceChangeType FromModificationType(this ModificationType type)
        {
            return type switch
            {
                ModificationType.Added => LookupServiceChangeType.Added,
                ModificationType.Modified => LookupServiceChangeType.Modified,
                ModificationType.Removed => LookupServiceChangeType.Removed,
                _ => LookupServiceChangeType.Initialized
            };
        }
    }
}
