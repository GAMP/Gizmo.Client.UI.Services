using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services
{
    internal static class RegistrationProfileMapper
    {
        internal static UserProfileModelCreate Map(RegistrationProfile source) =>
            new()
            {
                Username    = source.Username,
                FirstName   = source.FirstName,
                LastName    = source.LastName,
                BirthDate   = source.BirthDate,
                Sex         = source.Sex,
                Email       = source.Email,
                MobilePhone = source.MobilePhone,
                Phone       = source.Phone,
                Country     = source.Country,
                Address     = source.Address,
                City        = source.City,
                PostCode    = source.PostCode,
            };
    }
}
