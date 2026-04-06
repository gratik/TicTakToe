using System;
using System.Threading.Tasks;
using TicTakToe.App.Core.Services;
using Xunit;

public class CameraOverlayManagerTests
{
    [Fact]
    public async Task TriggerFlash_SetsAndClearsState()
    {
        var overlay = new CameraOverlayManager();
        overlay.TriggerFlash("flash!", "cue", 50, () => { });
        Assert.True(overlay.ShowCameraFlash);
        Assert.Equal("flash!", overlay.OverlayText);
        Assert.Equal("cue", overlay.CueMessage);
        await Task.Delay(100);
        Assert.False(overlay.ShowCameraFlash);
        Assert.Null(overlay.OverlayText);
    }

    [Fact]
    public async Task ShowVictory_SetsAndClearsState()
    {
        var overlay = new CameraOverlayManager();
        overlay.ShowVictory("victory!", "cue", 50, () => { });
        Assert.True(overlay.ShowVictoryHold);
        Assert.Equal("victory!", overlay.OverlayText);
        Assert.Equal("cue", overlay.CueMessage);
        await Task.Delay(100);
        Assert.False(overlay.ShowVictoryHold);
        Assert.Null(overlay.OverlayText);
    }

    [Fact]
    public void SetCueMessage_UpdatesCue()
    {
        var overlay = new CameraOverlayManager();
        bool stateChanged = false;
        overlay.SetCueMessage("new cue", () => stateChanged = true);
        Assert.Equal("new cue", overlay.CueMessage);
        Assert.True(stateChanged);
    }

    [Fact]
    public void Cancel_ResetsState()
    {
        var overlay = new CameraOverlayManager();
        overlay.TriggerFlash("flash", "cue", 100, () => { });
        overlay.Cancel();
        Assert.False(overlay.ShowCameraFlash);
        Assert.False(overlay.ShowVictoryHold);
        Assert.Null(overlay.OverlayText);
    }
}
