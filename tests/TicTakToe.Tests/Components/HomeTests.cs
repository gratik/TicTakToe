using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TicTakToe.App.Components.Pages;
using TicTakToe.App.Core.Models;

namespace TicTakToe.Tests.Components;

public class HomeTests : BunitContext
{
    private Func<Task>? _capturedHandler;
    private GameResult _engineResult = GameResult.InProgress;

    private (Mock<IGameEngine> engine, Mock<IStatsService> stats) SetupServices(
        Player currentPlayer = Player.X,
        GameMode mode = GameMode.PvP)
    {
        _engineResult = GameResult.InProgress;
        var board = new Board();
        var engineMock = new Mock<IGameEngine>();
        engineMock.Setup(e => e.Board).Returns(board);
        // Use lambda so tests can update _engineResult after render
        engineMock.Setup(e => e.Result).Returns(() => _engineResult);
        engineMock.Setup(e => e.CurrentPlayer).Returns(currentPlayer);
        engineMock.Setup(e => e.Mode).Returns(mode);
        engineMock.Setup(e => e.Difficulty).Returns(Difficulty.Medium);
        engineMock.SetupAdd(e => e.GameStateChanged += It.IsAny<Func<Task>>())
            .Callback<Func<Task>>(h => _capturedHandler = h);
        engineMock.SetupRemove(e => e.GameStateChanged -= It.IsAny<Func<Task>>());

        var statsMock = new Mock<IStatsService>();
        statsMock.Setup(s => s.GetStatsAsync(It.IsAny<GameMode>())).ReturnsAsync(GameStats.Empty);
        statsMock.Setup(s => s.ResetAllAsync()).Returns(Task.CompletedTask);
        statsMock.Setup(s => s.IncrementWinAsync(It.IsAny<GameMode>())).Returns(Task.CompletedTask);
        statsMock.Setup(s => s.IncrementLossAsync(It.IsAny<GameMode>())).Returns(Task.CompletedTask);
        statsMock.Setup(s => s.IncrementDrawAsync(It.IsAny<GameMode>())).Returns(Task.CompletedTask);

        Services.AddSingleton(engineMock.Object);
        Services.AddSingleton(statsMock.Object);
        Services.AddSingleton(Mock.Of<ILogger<Home>>());

        return (engineMock, statsMock);
    }

    private Task InvokeCapturedHandlerAsync() =>
        _capturedHandler?.Invoke() ?? Task.CompletedTask;

    // ── Initialisation ───────────────────────────────────────────────────────

    [Fact]
    public void OnInitialized_CallsStartGame_WithDefaultModeAndDifficulty()
    {
        var (engine, _) = SetupServices();

        Render<Home>();

        engine.Verify(e => e.StartGame(GameMode.PvP, Difficulty.Medium,
            It.Is<BoardConfiguration>(c => c.Size == 3 && c.WinLength == 3)), Times.Once);
    }

    [Fact]
    public void OnInitialized_SubscribesToGameStateChanged()
    {
        var (engine, _) = SetupServices();

        Render<Home>();

        engine.VerifyAdd(e => e.GameStateChanged += It.IsAny<Func<Task>>(), Times.Once);
    }

    // ── HandleHumanMove ──────────────────────────────────────────────────────

    [Fact]
    public void HandleHumanMove_CallsEngineHumanMove()
    {
        var (engine, _) = SetupServices();
        var cut = Render<Home>();

        cut.FindAll("button.cell")[4].Click();

        engine.Verify(e => e.HumanMove(4), Times.Once);
    }

    [Fact]
    public void HandleHumanMove_DoesNothing_WhenGameIsOver()
    {
        var (engine, _) = SetupServices();
        _engineResult = GameResult.XWins;
        var cut = Render<Home>();

        // Cells are disabled when game is over, so HumanMove should never be called
        engine.Verify(e => e.HumanMove(It.IsAny<int>()), Times.Never);
    }

    // ── HandleModeChanged ────────────────────────────────────────────────────

    [Fact]
    public void HandleModeChanged_UpdatesModeAndStartsNewGame()
    {
        var (engine, _) = SetupServices();
        var cut = Render<Home>();

        cut.Find("#mode-select").Change("PvC");

        engine.Verify(e => e.StartGame(GameMode.PvC, It.IsAny<Difficulty>(),
            It.Is<BoardConfiguration>(c => c.Size == 3 && c.WinLength == 3)), Times.Once);
    }

    [Fact]
    public void HandleModeChanged_ToCvC_StartsCvCGame()
    {
        var (engine, _) = SetupServices();
        var cut = Render<Home>();

        cut.Find("#mode-select").Change("CvC");

        engine.Verify(e => e.StartGame(GameMode.CvC, It.IsAny<Difficulty>(),
            It.Is<BoardConfiguration>(c => c.Size == 3 && c.WinLength == 3)), Times.Once);
    }

    // ── HandleDifficultyChanged ──────────────────────────────────────────────

    [Fact]
    public void HandleDifficultyChanged_UpdatesDifficultyAndStartsNewGame()
    {
        var (engine, _) = SetupServices(mode: GameMode.PvC);
        var cut = Render<Home>();

        cut.Find("#diff-select").Change("Hard");

        engine.Verify(e => e.StartGame(It.IsAny<GameMode>(), Difficulty.Hard,
            It.Is<BoardConfiguration>(c => c.Size == 3 && c.WinLength == 3)), Times.Once);
    }

    [Fact]
    public void HandleBoardSizeChanged_UpdatesSizeAndStartsNewGame()
    {
        var (engine, _) = SetupServices();
        var cut = Render<Home>();

        cut.Find("#board-size-select").Change("4");

        engine.Verify(e => e.StartGame(It.IsAny<GameMode>(), It.IsAny<Difficulty>(),
            It.Is<BoardConfiguration>(c => c.Size == 4 && c.WinLength == 4)), Times.AtLeastOnce());
    }

    // ── New Game button ──────────────────────────────────────────────────────

    [Fact]
    public void NewGame_Button_CallsStartGame()
    {
        var (engine, _) = SetupServices();
        var cut = Render<Home>();

        cut.Find("button.btn-primary").Click();

        // Called once on init, once on button click
        engine.Verify(e => e.StartGame(It.IsAny<GameMode>(), It.IsAny<Difficulty>(), It.IsAny<BoardConfiguration>()), Times.AtLeast(2));
    }

    // ── PersistResultAsync — PvP ─────────────────────────────────────────────

    [Fact]
    public async Task PersistResultAsync_PvP_Draw_CallsIncrementDraw()
    {
        var (_, stats) = SetupServices();
        Render<Home>();

        _engineResult = GameResult.Draw;
        await InvokeCapturedHandlerAsync();

        stats.Verify(s => s.IncrementDrawAsync(GameMode.PvP), Times.Once);
    }

    [Fact]
    public async Task PersistResultAsync_PvP_XWins_CallsIncrementWin()
    {
        var (_, stats) = SetupServices();
        Render<Home>();

        _engineResult = GameResult.XWins;
        await InvokeCapturedHandlerAsync();

        stats.Verify(s => s.IncrementWinAsync(GameMode.PvP), Times.Once);
    }

    [Fact]
    public async Task PersistResultAsync_PvP_OWins_CallsIncrementLoss()
    {
        var (_, stats) = SetupServices();
        Render<Home>();

        _engineResult = GameResult.OWins;
        await InvokeCapturedHandlerAsync();

        stats.Verify(s => s.IncrementLossAsync(GameMode.PvP), Times.Once);
    }

    // ── PersistResultAsync — PvC ─────────────────────────────────────────────

    [Fact]
    public async Task PersistResultAsync_PvC_XWins_CallsIncrementWin()
    {
        var (_, stats) = SetupServices();
        var cut = Render<Home>();
        cut.Find("#mode-select").Change("PvC"); // sets _mode = PvC in component

        _engineResult = GameResult.XWins;
        await InvokeCapturedHandlerAsync();

        stats.Verify(s => s.IncrementWinAsync(GameMode.PvC), Times.Once);
    }

    [Fact]
    public async Task PersistResultAsync_PvC_OWins_CallsIncrementLoss()
    {
        var (_, stats) = SetupServices();
        var cut = Render<Home>();
        cut.Find("#mode-select").Change("PvC");

        _engineResult = GameResult.OWins;
        await InvokeCapturedHandlerAsync();

        stats.Verify(s => s.IncrementLossAsync(GameMode.PvC), Times.Once);
    }

    [Fact]
    public async Task PersistResultAsync_PvC_Draw_CallsIncrementDraw()
    {
        var (_, stats) = SetupServices();
        var cut = Render<Home>();
        cut.Find("#mode-select").Change("PvC");

        _engineResult = GameResult.Draw;
        await InvokeCapturedHandlerAsync();

        stats.Verify(s => s.IncrementDrawAsync(GameMode.PvC), Times.Once);
    }

    // ── PersistResultAsync — CvC ─────────────────────────────────────────────

    [Fact]
    public async Task PersistResultAsync_CvC_XWins_CallsIncrementWin()
    {
        var (_, stats) = SetupServices();
        var cut = Render<Home>();
        cut.Find("#mode-select").Change("CvC");

        _engineResult = GameResult.XWins;
        await InvokeCapturedHandlerAsync();

        stats.Verify(s => s.IncrementWinAsync(GameMode.CvC), Times.Once);
    }

    [Fact]
    public async Task PersistResultAsync_CvC_Draw_CallsIncrementDraw()
    {
        var (_, stats) = SetupServices();
        var cut = Render<Home>();
        cut.Find("#mode-select").Change("CvC");

        _engineResult = GameResult.Draw;
        await InvokeCapturedHandlerAsync();

        stats.Verify(s => s.IncrementDrawAsync(GameMode.CvC), Times.Once);
    }

    // ── Dispose ──────────────────────────────────────────────────────────────

    [Fact]
    public void Dispose_UnsubscribesFromGameStateChanged()
    {
        var (engine, _) = SetupServices();
        var cut = Render<Home>();

        cut.Instance.Dispose();

        engine.VerifyRemove(e => e.GameStateChanged -= It.IsAny<Func<Task>>(), Times.Once);
    }

    [Fact]
    public async Task PersistResultAsync_IsIdempotent_WhenEventFiresTwice()
    {
        var (_, stats) = SetupServices();
        Render<Home>();

        _engineResult = GameResult.XWins;
        await InvokeCapturedHandlerAsync();
        await InvokeCapturedHandlerAsync(); // second fire — should not double-write

        stats.Verify(s => s.IncrementWinAsync(GameMode.PvP), Times.Once);
    }
}
