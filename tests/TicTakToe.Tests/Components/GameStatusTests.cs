using Bunit;
using Moq;
using TicTakToe.App.Components.Game;

namespace TicTakToe.Tests.Components;

public class GameStatusTests : BunitContext
{
    private static Mock<IGameEngine> CreateMock(
        GameResult result = GameResult.InProgress,
        Player currentPlayer = Player.X,
        GameMode mode = GameMode.PvP)
    {
        var mock = new Mock<IGameEngine>();
        mock.Setup(e => e.Result).Returns(result);
        mock.Setup(e => e.CurrentPlayer).Returns(currentPlayer);
        mock.Setup(e => e.Mode).Returns(mode);
        return mock;
    }

    // ── StatusText ──────────────────────────────────────────────────────────

    [Fact]
    public void StatusText_ShowsXWins_WhenXWins()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.XWins).Object));

        Assert.Contains("Player X wins", cut.Find(".status-text").TextContent);
    }

    [Fact]
    public void StatusText_ShowsOWins_WhenOWins()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.OWins).Object));

        Assert.Contains("Player O wins", cut.Find(".status-text").TextContent);
    }

    [Fact]
    public void StatusText_ShowsDraw_WhenDraw()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.Draw).Object));

        Assert.Contains("draw", cut.Find(".status-text").TextContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StatusText_ShowsPlayerXTurn_InPvP_WhenXTurn()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.InProgress, Player.X, GameMode.PvP).Object));

        Assert.Contains("Player X", cut.Find(".status-text").TextContent);
    }

    [Fact]
    public void StatusText_ShowsPlayerOTurn_InPvP_WhenOTurn()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.InProgress, Player.O, GameMode.PvP).Object));

        Assert.Contains("Player O", cut.Find(".status-text").TextContent);
    }

    [Fact]
    public void StatusText_ShowsYourTurn_InPvC_WhenXTurn()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.InProgress, Player.X, GameMode.PvC).Object));

        Assert.Contains("Your", cut.Find(".status-text").TextContent);
    }

    [Fact]
    public void StatusText_ShowsComputerTurn_InPvC_WhenOTurn()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.InProgress, Player.O, GameMode.PvC).Object));

        Assert.Contains("Computer", cut.Find(".status-text").TextContent);
    }

    [Fact]
    public void StatusText_ShowsComputerThinking_InCvC()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.InProgress, Player.X, GameMode.CvC).Object));

        Assert.Contains("thinking", cut.Find(".status-text").TextContent, StringComparison.OrdinalIgnoreCase);
    }

    // ── StatusIcon ───────────────────────────────────────────────────────────

    [Fact]
    public void StatusIcon_IsTrophy_WhenXWins()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.XWins).Object));

        Assert.Contains("🏆", cut.Find(".status-icon").TextContent);
    }

    [Fact]
    public void StatusIcon_IsTrophy_WhenOWins()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.OWins).Object));

        Assert.Contains("🏆", cut.Find(".status-icon").TextContent);
    }

    [Fact]
    public void StatusIcon_IsHandshake_WhenDraw()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.Draw).Object));

        Assert.Contains("🤝", cut.Find(".status-icon").TextContent);
    }

    [Fact]
    public void StatusIcon_IsX_WhenXTurn()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.InProgress, Player.X).Object));

        Assert.Contains("✖️", cut.Find(".status-icon").TextContent);
    }

    [Fact]
    public void StatusIcon_IsO_WhenOTurn()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.InProgress, Player.O).Object));

        Assert.Contains("⭕", cut.Find(".status-icon").TextContent);
    }

    // ── StatusClass ──────────────────────────────────────────────────────────

    [Fact]
    public void StatusClass_IsStatusWinX_WhenXWins()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.XWins).Object));

        var classes = cut.Find(".status").ClassList;
        Assert.Contains("status-win", classes);
        Assert.Contains("status-x", classes);
    }

    [Fact]
    public void StatusClass_IsStatusWinO_WhenOWins()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.OWins).Object));

        var classes = cut.Find(".status").ClassList;
        Assert.Contains("status-win", classes);
        Assert.Contains("status-o", classes);
    }

    [Fact]
    public void StatusClass_IsStatusDraw_WhenDraw()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.Draw).Object));

        Assert.Contains("status-draw", cut.Find(".status").ClassList);
    }

    [Fact]
    public void StatusClass_IsStatusPlaying_WhenInProgress()
    {
        var cut = Render<GameStatus>(p => p
            .Add(s => s.Engine, CreateMock(GameResult.InProgress).Object));

        Assert.Contains("status-playing", cut.Find(".status").ClassList);
    }
}
