using System;
using System.Threading;
using System.Threading.Tasks;

namespace TicTakToe.App.Core.Services;

/// <summary>
/// Handles scheduling and cancellation of AI moves for CvC and PvC modes.
/// </summary>
public sealed class CvcScheduler : IDisposable
{
    private CancellationTokenSource? _cts;

    public void Schedule(int delayMs, Func<Task> action)
    {
        Cancel();
        _cts = new CancellationTokenSource();
        // Fire-and-forget: intentionally not awaited. Cancellation is handled via _cts.
        // This is safe for UI/game scheduling where completion is not required by the caller.
        _ = ExecuteAfterDelayAsync(delayMs, _cts.Token, action);
    }

    public void Cancel()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private static async Task ExecuteAfterDelayAsync(int delayMs, CancellationToken token, Func<Task> action)
    {
        try { await Task.Delay(delayMs, token); } catch (OperationCanceledException) { return; }
        await action();
    }

    public void Dispose() => Cancel();
}
