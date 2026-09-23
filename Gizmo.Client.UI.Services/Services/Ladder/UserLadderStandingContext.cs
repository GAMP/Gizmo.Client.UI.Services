namespace Gizmo.Client.UI.Services;

public sealed class UserLadderStandingContext : IUserLadderStandingContext
{
    private readonly IUserLadderService _ladderService;

    // guards against a refresh in flight at Clear() repopulating the context afterwards
    private int _generation;

    public UserLadderStandingContext(IUserLadderService ladderService)
    {
        _ladderService = ladderService;
    }

    public event EventHandler? Changed;

    public UserLadderStanding? Standing { get; private set; }

    public async Task RefreshAsync(CancellationToken ct = default)
    {
        var generation = _generation;

        var standing = await _ladderService.GetStandingAsync(ct);

        if (generation != _generation)
            return;

        Standing = standing;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Clear()
    {
        _generation++;
        Standing = null;
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
