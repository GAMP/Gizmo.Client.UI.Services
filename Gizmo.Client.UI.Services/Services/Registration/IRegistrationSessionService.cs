namespace Gizmo.Client.UI.Services;

public interface IRegistrationSessionService
{
    event EventHandler? Changed;

    string Token { get; }
    string Destination { get; }
    int CodeLength { get; }
    RegistrationFlow Flow { get; }

    void SetStartResult(string token, string destination, int codeLength, RegistrationFlow flow);
    void Clear();
}
