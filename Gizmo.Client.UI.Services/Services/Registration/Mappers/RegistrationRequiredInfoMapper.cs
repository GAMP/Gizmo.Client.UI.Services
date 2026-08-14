using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services
{
    internal static class RegistrationRequiredInfoMapper
    {
        internal static RegistrationRequiredInfo Map(UserModelRequiredInfo source) =>
            new()
            {
                FirstName  = source.FirstName,
                LastName   = source.LastName,
                BirthDate  = source.BirthDate,
                Address    = source.Address,
                City       = source.City,
                PostCode   = source.PostCode,
                State      = source.State,
                Country    = source.Country,
                Email      = source.Email,
                Phone      = source.Phone,
                Mobile     = source.Mobile,
                Sex        = source.Sex,
                Password   = true,
            };
    }
}
