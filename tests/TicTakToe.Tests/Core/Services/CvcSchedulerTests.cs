using System;
using System.Threading.Tasks;
using TicTakToe.App.Core.Services;
using Xunit;

public class CvcSchedulerTests
{
    [Fact]
    public async Task Schedule_ExecutesActionAfterDelay()
    {
        var scheduler = new CvcScheduler();
        bool ran = false;
        scheduler.Schedule(50, () => { ran = true; return Task.CompletedTask; });
        await Task.Delay(100);
        Assert.True(ran);
    }

    [Fact]
    public async Task Cancel_PreventsScheduledAction()
    {
        var scheduler = new CvcScheduler();
        bool ran = false;
        scheduler.Schedule(100, () => { ran = true; return Task.CompletedTask; });
        scheduler.Cancel();
        await Task.Delay(150);
        Assert.False(ran);
    }
}
