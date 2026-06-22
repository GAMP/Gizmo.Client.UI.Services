using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

internal static class RegistrationCompleteRequestMapper
{
    public static Gizmo.Web.Api.Models.RegistrationCompleteModel MapComplete(RegistrationCompleteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new Gizmo.Web.Api.Models.RegistrationCompleteModel
        {
            Token          = request.Token,
            Password       = request.Password,
            Profile        = RegistrationProfileMapper.Map(request.Profile),
            AgreementStates = MapAgreementStates(request.AgreementStates)
        };
    }

    public static Gizmo.Web.Api.Models.RegistrationDirectModel MapDirect(RegistrationCompleteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new Gizmo.Web.Api.Models.RegistrationDirectModel
        {
            Password       = request.Password,
            Profile        = RegistrationProfileMapper.Map(request.Profile),
            AgreementStates = MapAgreementStates(request.AgreementStates)
        };
    }

    private static List<UserAgreementModelState> MapAgreementStates(
        IReadOnlyList<RegistrationAgreementChoice> choices)
        => choices is null
            ? new()
            : choices.Select(c => new UserAgreementModelState
            {
                UserId = 0,
                UserAgreementId = c.AgreementId,
                AcceptState = c.AcceptState
            }).ToList();
}
