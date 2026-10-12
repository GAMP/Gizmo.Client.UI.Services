namespace Gizmo.Client.UI.Services;

internal static class UserProfileUpdateMapper
{
    internal static Web.Api.Models.UserProfileModelUserUpdate Map(UserProfileUpdate source) =>
        new()
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            BirthDate = source.BirthDate,
            Sex = source.Sex.HasValue ? UserProfileSexMapper.ToModel(source.Sex.Value) : null,
            Email = source.Email,
            MobilePhone = source.MobilePhone,
            Phone = source.Phone,
            Address = source.Address,
            City = source.City,
            PostCode = source.PostCode,
            Country = source.Country,
        };
}
