namespace Gizmo.Client.UI.Services;

internal static class PasswordRecoveryStartResultMapper
{
    internal static PasswordRecoveryStartResult Map(Gizmo.Web.Api.Models.VerificationStartResultModel model, string matchValue)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new PasswordRecoveryStartResult(
            Result:           PasswordRecoveryStartCodeMapper.Map(model.Result),
            Token:            model.Token,
            Destination:      ComputeDestination(matchValue),
            CodeLength:       model.CodeLength,
            ExpiresInSeconds: model.ExpiresInSeconds);
    }

    private static string ComputeDestination(string matchValue)
    {
        if (matchValue.Contains('@'))
            return MaskEmail(matchValue);

        var digits = new string(matchValue.TrimStart('+').Where(char.IsDigit).ToArray());
        if (digits.Length > 0 && digits.Length >= matchValue.TrimStart('+').Length / 2)
            return MaskPhone(matchValue);

        return matchValue;
    }

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
