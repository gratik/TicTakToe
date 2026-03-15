using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TicTakToe.App.Components.Game;
using TicTakToe.App.Core.Models;
using TicTakToe.App.Core.Services.Interfaces;

namespace TicTakToe.Tests.Components;

public class GameBoardTests : BunitContext
{
    private Mock<IGameEngine> CreateEngineMock(
        GameMode mode = GameMode.PvP,
        Player currentPlayer = Player.X,
        GameResult result = GameResult.InProgress)
    {
        var board = new Board();
        var mock = new Mock<IGameEngine>();
        mock.Setup(e => e.Board).Returns(board);
        mock.Setup(e => e.Mode).Returns(mode);
        mock.Setup(e => e.CurrentPlayer).Returns(currentPlayer);
        mock.Setup(e => e.Result).Returns(result);
        return mock;
    }

    [Fact]
    public void Renders_NineCells()
    {
        var engineMock = CreateEngineMock();
        var cut = Render<GameBoard>(p => p
            .Add(b => b.Engine, engineMock.Object));

        Assert.Equal(9, cut.FindAll("button.cell").Count);
    }

    [Fact]
    public void Cells_AreClickable_InPvP_Mode()
    {
        var engineMock = CreateEngineMock(GameMode.PvP);
        var cut = Render<GameBoard>(p => p
            .Add(b => b.Engine, engineMock.Object));

        var buttons = cut.FindAll("button.cell");
        Assert.All(buttons, btn => Assert.False(btn.HasAttribute("disabled")));
    }

    [Fact]
    public void Cells_AreNotClickable_InCvC_Mode()
    {
        var engineMock = CreateEngineMock(GameMode.CvC);
        var cut = Render<GameBoard>(p => p
            .Add(b => b.Engine, engineMock.Object));

        var buttons = cut.FindAll("button.cell");
        Assert.All(buttons, btn => Assert.True(btn.HasAttribute("disabled")));
    }

    [Fact]
    public void Cells_AreClickable_InPvC_WhenXTurn()
    {
        var engineMock = CreateEngineMock(GameMode.PvC, Player.X);
        var cut = Render<GameBoard>(p => p
            .Add(b => b.Engine, engineMock.Object));

        var buttons = cut.FindAll("button.cell");
        Assert.All(buttons, btn => Assert.False(btn.HasAttribute("disabled")));
    }

    [Fact]
    public void Cells_AreNotClickable_InPvC_WhenOTurn()
    {
        var engineMock = CreateEngineMock(GameMode.PvC, Player.O);
        var cut = Render<GameBoard>(p => p
            .Add(b => b.Engine, engineMock.Object));

        var buttons = cut.FindAll("button.cell");
        Assert.All(buttons, btn => Assert.True(btn.HasAttribute("disabled")));
    }

    [Fact]
    public void Clicking_Cell_Fires_OnHumanMove_WithCorrectIndex()
    {
        var engineMock = CreateEngineMock(GameMode.PvP);
        int? receivedIndex = null;

        var cut = Render<GameBoard>(p => p
            .Add(b => b.Engine, engineMock.Object)
            .Add(b => b.OnHumanMove, EventCallback.Factory.Create<int>(this, (int idx) => receivedIndex = idx)));

        cut.FindAll("button.cell")[3].Click();

        Assert.Equal(3, receivedIndex);
    }

    [Fact]
    public void WinningCells_HaveCellWinClass_AfterGameEnds()
    {
        var board = new Board();
        board.MakeMove(0, Player.X);
        board.MakeMove(1, Player.X);
        board.MakeMove(2, Player.X);

        var mock = new Mock<IGameEngine>();
        mock.Setup(e => e.Board).Returns(board);
        mock.Setup(e => e.Mode).Returns(GameMode.PvP);
        mock.Setup(e => e.CurrentPlayer).Returns(Player.O);
        mock.Setup(e => e.Result).Returns(GameResult.XWins);

        var cut = Render<GameBoard>(p => p
            .Add(b => b.Engine, mock.Object));

        var cells = cut.FindAll("button.cell");
        Assert.Contains("cell-win", cells[0].ClassList);
        Assert.Contains("cell-win", cells[1].ClassList);
        Assert.Contains("cell-win", cells[2].ClassList);
        Assert.DoesNotContain("cell-win", cells[3].ClassList);
    }
}
