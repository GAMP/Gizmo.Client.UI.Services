using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services;

public sealed class CountdownTimer : IDisposable
{
    private CancellationTokenSource? _cts;

    public async Task StartAsync(int seconds, Func<int, Task> onTick, ILogger? logger = null)
    {
        Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;
        var remaining = seconds;
        await onTick(remaining);

        try
        {
            while (remaining > 0 && !token.IsCancellationRequested)
            {
                await Task.Delay(1000, token);
                if (token.IsCancellationRequested) break;
                remaining--;
                await onTick(remaining);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Countdown timer faulted.");
        }
    }

    public void Cancel()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public void Dispose() => Cancel();
}
