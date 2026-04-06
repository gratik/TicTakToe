    // (removed stray duplicate SetCueMessage)
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TicTakToe.App.Core.Services
{
/// <summary>
/// Manages camera overlay state and timing for cinematic effects.
/// </summary>
public sealed class CameraOverlayManager : IDisposable
{
    private CancellationTokenSource? _cts;
    public bool ShowCameraFlash { get; private set; }
    public bool ShowVictoryHold { get; private set; }
    public string? OverlayText { get; private set; }
    public string? CueMessage { get; private set; }

    public void SetCueMessage(string cueMessage, Action stateChanged)
    {
        CueMessage = cueMessage;
        stateChanged();
    }

    public void TriggerFlash(string overlayText, string cueMessage, int durationMs, Action stateChanged)
    {
        Cancel();
        ShowVictoryHold = false;
        ShowCameraFlash = true;
        OverlayText = overlayText;
        CueMessage = cueMessage;
        stateChanged();
        _cts = new CancellationTokenSource();
        // Fire-and-forget: intentionally not awaited. Cancellation is handled via _cts.
        // This is safe for UI/game overlay timing where completion is not required by the caller.
        _ = ExecuteAfterDelayAsync(durationMs, _cts.Token, () =>
        {
            ShowCameraFlash = false;
            OverlayText = null;
            stateChanged();
            return Task.CompletedTask;
        });
    }

    public void ShowVictory(string overlayText, string cueMessage, int durationMs, Action stateChanged)
    {
        Cancel();
        ShowCameraFlash = false;
        ShowVictoryHold = true;
        OverlayText = overlayText;
        CueMessage = cueMessage;
        stateChanged();
        _cts = new CancellationTokenSource();
        // Fire-and-forget: intentionally not awaited. Cancellation is handled via _cts.
        // This is safe for UI/game overlay timing where completion is not required by the caller.
        _ = ExecuteAfterDelayAsync(durationMs, _cts.Token, () =>
        {
            ShowVictoryHold = false;
            OverlayText = null;
            stateChanged();
            return Task.CompletedTask;
        });
    }

    public void Cancel()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        ShowCameraFlash = false;
        ShowVictoryHold = false;
        OverlayText = null;
    }

    private static async Task ExecuteAfterDelayAsync(int delayMs, CancellationToken token, Func<Task> action)
    {
        try { await Task.Delay(delayMs, token); } catch (OperationCanceledException) { return; }
        await action();
    }

    public void Dispose() => Cancel();
}
}
