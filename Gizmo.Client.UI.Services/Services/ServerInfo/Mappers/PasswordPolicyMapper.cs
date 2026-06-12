using Gizmo.Server.Options;

namespace Gizmo.Client.UI.Services
{
    internal static class PasswordPolicyMapper
    {
        internal static PasswordPolicy Map(UserPasswordPolicyOptions source) =>
            new()
            {
                MinimumLength    = source.MinimumLength,
                MaximumLength    = source.MaximumLength,
                RequireLowerCase = source.RequireLowerCase,
                RequireUpperCase = source.RequireUpperCase,
                RequireNumbers   = source.RequireNumbers,
            };
    }
}
