using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services
{
    public sealed class RegistrationProfile
    {
        public string? Username { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public DateTime? BirthDate { get; init; }
        public Sex Sex { get; init; }
        public string? Email { get; init; }
        public string? MobilePhone { get; init; }
        public string? Phone { get; init; }
        public string? Country { get; init; }
        public string? Address { get; init; }
        public string? City { get; init; }
        public string? PostCode { get; init; }
    }
}
