namespace Gizmo.Client.UI.Services
{
    public sealed class PasswordPolicy
    {
        public int MinimumLength { get; init; }
        public int MaximumLength { get; init; }
        public bool RequireLowerCase { get; init; }
        public bool RequireUpperCase { get; init; }
        public bool RequireNumbers { get; init; }
    }
}
