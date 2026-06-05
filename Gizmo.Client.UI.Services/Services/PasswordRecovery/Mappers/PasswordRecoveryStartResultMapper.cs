namespace Gizmo.Client.UI.Services;

internal static class PasswordRecoveryStartResultMapper
{
    internal static PasswordRecoveryStartResult Map(Gizmo.Web.Api.Models.VerificationStartResultModel model, string matchValue, PasswordRecoveryChannel channel)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new PasswordRecoveryStartResult(
            Result:           PasswordRecoveryStartCodeMapper.Map(model.Result),
            Token:            model.Token,
            Destination:      ComputeDestination(matchValue, channel),
            CodeLength:       model.CodeLength,
            ExpiresInSeconds: model.ExpiresInSeconds);
    }

    private static string ComputeDestination(string matchValue, PasswordRecoveryChannel channel) =>
        channel == PasswordRecoveryChannel.Email ? MaskEmail(matchValue) : MaskPhone(matchValue);

    private static string MaskEmail(string email)
    {
        var at = email.IndexOf('@');
        if (at <= 1) return email;
        return email[0] + "***" + email[at..];
    }

    private static string MaskPhone(string phone)
    {
        var digits = new string(phone.TrimStart('+').Where(char.IsDigit).ToArray());
        if (digits.Length <= 4) return digits;
        return "****" + digits[^4..];
    }
}
