namespace Gizmo.Client.UI.Services;

internal static class UserProfileSexMapper
{
    internal static Web.Api.Models.Sex ToModel(UserProfileSex source) => source switch
    {
        UserProfileSex.Male => Web.Api.Models.Sex.Male,
        UserProfileSex.Female => Web.Api.Models.Sex.Female,
        _ => Web.Api.Models.Sex.Unspecified,
    };

    internal static UserProfileSex FromModel(Web.Api.Models.Sex source) => source switch
    {
        Web.Api.Models.Sex.Male => UserProfileSex.Male,
        Web.Api.Models.Sex.Female => UserProfileSex.Female,
        _ => UserProfileSex.Unspecified,
    };
}
