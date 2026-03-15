namespace TicTakToe.Tests.Core;

public class GameStatsTests
{
    [Fact]
    public void Empty_HasAllZeroes()
    {
        Assert.Equal(0, GameStats.Empty.Wins);
        Assert.Equal(0, GameStats.Empty.Losses);
        Assert.Equal(0, GameStats.Empty.Draws);
        Assert.Equal(0, GameStats.Empty.Total);
    }

    [Fact]
    public void WithWin_IncrementsWins_OnlyWins()
    {
        var stats = GameStats.Empty.WithWin();
        Assert.Equal(1, stats.Wins);
        Assert.Equal(0, stats.Losses);
        Assert.Equal(0, stats.Draws);
    }

    [Fact]
    public void WithLoss_IncrementsLosses()
    {
        var stats = GameStats.Empty.WithLoss();
        Assert.Equal(1, stats.Losses);
    }

    [Fact]
    public void WithDraw_IncrementsDraw()
    {
        var stats = GameStats.Empty.WithDraw();
        Assert.Equal(1, stats.Draws);
    }

    [Fact]
    public void Total_SumsAllCounters()
    {
        var stats = new GameStats(3, 2, 1);
        Assert.Equal(6, stats.Total);
    }

    [Fact]
    public void With_Methods_AreImmutable()
    {
        var original = GameStats.Empty;
        _ = original.WithWin();
        Assert.Equal(0, original.Wins); // original unchanged
    }
}
