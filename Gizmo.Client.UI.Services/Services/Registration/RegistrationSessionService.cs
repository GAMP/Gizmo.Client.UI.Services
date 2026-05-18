namespace Gizmo.Client.UI.Services;

public sealed class RegistrationSessionService : IRegistrationSessionService
{
    public event EventHandler? Changed;

    public string Token { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public int CodeLength { get; private set; }
    public RegistrationFlow Flow { get; private set; }

    public void SetStartResult(string token, string destination, int codeLength, RegistrationFlow flow)
    {
        Token = token;
        Destination = destination;
        CodeLength = codeLength;
        Flow = flow;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Clear()
    {
        Token = string.Empty;
        Destination = string.Empty;
        CodeLength = 0;
        Flow = RegistrationFlow.None;
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
