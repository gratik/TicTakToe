using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using TicTakToe.App.Core.Models;

namespace TicTakToe.Tests.Core;

public class GameEngineTests
{
    private GameEngine CreateEngine(Difficulty difficulty = Difficulty.Hard)
    {
        var strategies = new IAiStrategy[]
        {
            new RandomStrategy(),
            new WeightedStrategy(),
            new MinimaxStrategy()
        };
        var ai = new AiPlayer(strategies);
        var logger = Mock.Of<ILogger<GameEngine>>();
        return new GameEngine(ai, logger);
    }

    [Fact]
    public void StartGame_ResetsBoard_AndSetsCurrentPlayerX()
    {
        var engine = CreateEngine();
        engine.StartGame(GameMode.PvP, Difficulty.Easy, BoardConfiguration.Default);
        Assert.Equal(Player.X, engine.CurrentPlayer);
        Assert.Equal(GameResult.InProgress, engine.Result);
        Assert.All(engine.Board.Cells, c => Assert.Equal(Player.None, c));
    }

    [Fact]
    public void StartGame_RaisesGameStateChanged()
    {
        var engine = CreateEngine();
        bool raised = false;
        engine.GameStateChanged += () =>
        {
            raised = true;
            return Task.CompletedTask;
        };
        engine.StartGame(GameMode.PvP, Difficulty.Easy, BoardConfiguration.Default);
        Assert.True(raised);
    }

    [Fact]
    public void HumanMove_PlacesCurrentPlayer_AndSwitchesTurn()
    {
        var engine = CreateEngine();
        engine.StartGame(GameMode.PvP, Difficulty.Easy, BoardConfiguration.Default);
        engine.HumanMove(0);
        Assert.Equal(Player.X, engine.Board[0]);
        Assert.Equal(Player.O, engine.CurrentPlayer);
    }

    [Fact]
    public void HumanMove_DoesNothing_OnOccupiedCell()
    {
        var engine = CreateEngine();
        engine.StartGame(GameMode.PvP, Difficulty.Easy, BoardConfiguration.Default);
        engine.HumanMove(0);
        var playerBefore = engine.CurrentPlayer;
        engine.HumanMove(0); // occupied
        Assert.Equal(playerBefore, engine.CurrentPlayer);
    }

    [Fact]
    public void HumanMove_DoesNothing_WhenGameOver()
    {
        var engine = CreateEngine();
        engine.StartGame(GameMode.PvP, Difficulty.Easy, BoardConfiguration.Default);
        // Force X win
        engine.HumanMove(0); engine.HumanMove(3);
        engine.HumanMove(1); engine.HumanMove(4);
        engine.HumanMove(2); // X wins top row

        Assert.Equal(GameResult.XWins, engine.Result);
        engine.HumanMove(5); // should no-op
        Assert.Equal(Player.None, engine.Board[5]);
    }

    [Fact]
    public void HumanMove_RaisesGameStateChanged()
    {
        var engine = CreateEngine();
        engine.StartGame(GameMode.PvP, Difficulty.Easy, BoardConfiguration.Default);
        int count = 0;
        engine.GameStateChanged += () =>
        {
            count++;
            return Task.CompletedTask;
        };
        engine.HumanMove(0);
        Assert.Equal(1, count);
    }

    [Fact]
    public void TriggerAiMove_PlacesMove_ForCurrentPlayer()
    {
        var engine = CreateEngine();
        engine.StartGame(GameMode.PvC, Difficulty.Hard, BoardConfiguration.Default);
        engine.TriggerAiMove(); // AI plays as X first
        Assert.NotEqual(Player.None, engine.Board.Cells.First(c => c != Player.None));
    }

    [Fact]
    public void TriggerAiMove_DoesNothing_WhenGameOver()
    {
        var engine = CreateEngine();
        engine.StartGame(GameMode.PvP, Difficulty.Easy, BoardConfiguration.Default);
        engine.HumanMove(0); engine.HumanMove(3);
        engine.HumanMove(1); engine.HumanMove(4);
        engine.HumanMove(2); // X wins

        var boardSnapshot = engine.Board.Cells.ToArray();
        engine.TriggerAiMove(); // should no-op
        Assert.Equal(boardSnapshot, engine.Board.Cells);
    }

    [Fact]
    public void StartGame_SetsCorrectModeAndDifficulty()
    {
        var engine = CreateEngine();
        engine.StartGame(GameMode.CvC, Difficulty.Medium, BoardConfiguration.Default);
        Assert.Equal(GameMode.CvC, engine.Mode);
        Assert.Equal(Difficulty.Medium, engine.Difficulty);
    }

    [Fact]
    public void StartGame_AppliesCustomBoardConfiguration()
    {
        var engine = CreateEngine();
        var custom = BoardConfiguration.FourByFour;
        engine.StartGame(GameMode.PvP, Difficulty.Easy, custom);
        Assert.Equal(4, engine.Board.Size);
        Assert.Equal(custom, engine.CurrentConfiguration);
    }

    [Fact]
    public void HumanMove_IsNoOp_InCvCMode()
    {
        var engine = CreateEngine();
        engine.StartGame(GameMode.CvC, Difficulty.Easy, BoardConfiguration.Default);

        engine.HumanMove(0);

        Assert.Equal(Player.None, engine.Board[0]);
    }

    [Fact]
    public void TriggerAiMove_IsNoOp_InPvPMode()
    {
        var engine = CreateEngine();
        engine.StartGame(GameMode.PvP, Difficulty.Easy, BoardConfiguration.Default);

        var snapshot = engine.Board.Cells.ToArray();
        engine.TriggerAiMove();

        Assert.Equal(snapshot, engine.Board.Cells);
    }
}
