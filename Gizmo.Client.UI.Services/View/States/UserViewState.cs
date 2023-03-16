using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserViewState : ViewStateBase
    {
        #region PROPERTIES

        public int Id { get; internal set; }

        public string? Username { get; internal set; }

        public string? FirstName { get; internal set; }

        public string? LastName { get; internal set; }

        public DateTime? BirthDate { get; internal set; }

        public Sex Sex { get; internal set; }

        public string? Country { get; internal set; }

        public string? Address { get; internal set; }

        public string? Email { get; internal set; }

        public string? Phone { get; internal set; }

        public string? MobilePhone { get; internal set; }

        public DateTime RegistrationDate { get; internal set; }

        public string? Picture { get; internal set; }

        #endregion
    }
}
