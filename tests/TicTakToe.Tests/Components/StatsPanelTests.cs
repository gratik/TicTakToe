using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TicTakToe.App.Components.Stats;
using TicTakToe.App.Core.Models;
using TicTakToe.App.Core.Services.Interfaces;

namespace TicTakToe.Tests.Components;

public class StatsPanelTests : BunitContext
{
    private Mock<IStatsService> CreateStatsMock(
        GameStats? pvp = null,
        GameStats? pvc = null,
        GameStats? cvc = null)
    {
        var mock = new Mock<IStatsService>();
        mock.Setup(s => s.GetStatsAsync(GameMode.PvP)).ReturnsAsync(pvp ?? GameStats.Empty);
        mock.Setup(s => s.GetStatsAsync(GameMode.PvC)).ReturnsAsync(pvc ?? GameStats.Empty);
        mock.Setup(s => s.GetStatsAsync(GameMode.CvC)).ReturnsAsync(cvc ?? GameStats.Empty);
        mock.Setup(s => s.ResetAllAsync()).Returns(Task.CompletedTask);
        return mock;
    }

    [Fact]
    public void Renders_RowForEachGameMode()
    {
        var mock = CreateStatsMock();
        Services.AddSingleton(mock.Object);

        var cut = Render<StatsPanel>(p => p
            .Add(s => s.ActiveMode, GameMode.PvP));

        Assert.Equal(3, cut.FindAll(".stats-row").Count);
    }

    [Fact]
    public void Displays_Correct_WinLossDraw_Numbers()
    {
        var pvpStats = new GameStats(5, 3, 2);
        var mock = CreateStatsMock(pvp: pvpStats);
        Services.AddSingleton(mock.Object);

        var cut = Render<StatsPanel>(p => p
            .Add(s => s.ActiveMode, GameMode.PvP));

        var html = cut.Markup;
        Assert.Contains("W: 5", html);
        Assert.Contains("L: 3", html);
        Assert.Contains("D: 2", html);
    }

    [Fact]
    public void ActiveMode_Row_HasActiveClass()
    {
        var mock = CreateStatsMock();
        Services.AddSingleton(mock.Object);

        var cut = Render<StatsPanel>(p => p
            .Add(s => s.ActiveMode, GameMode.PvC));

        var rows = cut.FindAll(".stats-row");
        // PvP=0, PvC=1, CvC=2
        Assert.Contains("stats-row-active", rows[1].ClassList);
        Assert.DoesNotContain("stats-row-active", rows[0].ClassList);
        Assert.DoesNotContain("stats-row-active", rows[2].ClassList);
    }

    [Fact]
    public async Task Reset_Button_Calls_ResetAllAsync_And_Refreshes()
    {
        var mock = CreateStatsMock(pvp: new GameStats(2, 1, 0));
        mock.Setup(s => s.ResetAllAsync())
            .Callback(() =>
            {
                mock.Setup(s => s.GetStatsAsync(GameMode.PvP)).ReturnsAsync(GameStats.Empty);
                mock.Setup(s => s.GetStatsAsync(GameMode.PvC)).ReturnsAsync(GameStats.Empty);
                mock.Setup(s => s.GetStatsAsync(GameMode.CvC)).ReturnsAsync(GameStats.Empty);
            })
            .Returns(Task.CompletedTask);
        Services.AddSingleton(mock.Object);

        var cut = Render<StatsPanel>(p => p
            .Add(s => s.ActiveMode, GameMode.PvP));

        await cut.Find("button.btn-danger").ClickAsync(new());

        mock.Verify(s => s.ResetAllAsync(), Times.Once);
        Assert.Contains("W: 0", cut.Markup);
    }

    [Fact]
    public async Task Reset_Button_Invokes_OnReset_Callback()
    {
        var mock = CreateStatsMock();
        Services.AddSingleton(mock.Object);

        bool callbackFired = false;
        var cut = Render<StatsPanel>(p => p
            .Add(s => s.ActiveMode, GameMode.PvP)
            .Add(s => s.OnReset, EventCallback.Factory.Create(this, () => callbackFired = true)));

        await cut.Find("button.btn-danger").ClickAsync(new());

        Assert.True(callbackFired);
    }
}
