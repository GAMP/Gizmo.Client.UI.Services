namespace Gizmo.Client.UI.Services;

internal static class RegistrationStartResultMapper
{
    public static RegistrationStartResult Map(Gizmo.Web.Api.Models.VerificationStartResultModel model,
                                              RegistrationStartRequest request)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(request);
        return new RegistrationStartResult(
            Result:           RegistrationStartCodeMapper.Map(model.Result),
            Token:            model.Token,
            Destination:      ComputeDestination(request),
            CodeLength:       model.CodeLength,
            DeliveryMethod:   null,  // R5: API не возвращает DeliveryMethod
            RedirectUrl:      model.RedirectUrl,
            ExpiresInSeconds: model.ExpiresInSeconds
        );
    }

    private static string? ComputeDestination(RegistrationStartRequest request) =>
        request.DeliveryMethod switch
        {
            RegistrationDeliveryMethod.Redirect => null,
            _ when request.Email != null        => MaskEmail(request.Email),
            _ when request.Phone != null        => MaskPhone(request.Phone),
            _                                   => null
        };

    private static string MaskEmail(string email)
    {
        var at = email.IndexOf('@');
        if (at <= 1) return email;
        return email[0] + "***" + email[at..];
    }

    private static string MaskPhone(string phone)
    {
        var digits = phone.TrimStart('+');
        if (digits.Length <= 4) return digits;
        return "****" + digits[^4..];
    }
}
