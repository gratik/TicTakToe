using System.Linq;
using TicTakToe.App.Core.Models;
using TicTakToe.App.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace TicTakToe.App.Core.Services;

/// <summary>
/// Orchestrates turns, enforces rules, and raises state-change events
/// for a single Tic-Tac-Toe game session.
/// </summary>
public sealed class GameEngine : IGameEngine
{
    private readonly IAiPlayer _aiPlayer;
    private readonly ILogger<GameEngine> _logger;

    /// <summary>Initialises a new <see cref="GameEngine"/> with the supplied AI player.</summary>
    public GameEngine(IAiPlayer aiPlayer, ILogger<GameEngine> logger)
    {
        _aiPlayer = aiPlayer;
        _logger = logger;
        Board = new Board();
        CurrentConfiguration = BoardConfiguration.Default;
    }

    /// <inheritdoc/>
    public Board Board { get; private set; }

    /// <inheritdoc/>
    public Player CurrentPlayer { get; private set; } = Player.X;

    /// <inheritdoc/>
    public GameResult Result { get; private set; } = GameResult.InProgress;

    /// <inheritdoc/>
    public GameMode Mode { get; private set; } = GameMode.PvP;

    /// <inheritdoc/>
    public Difficulty Difficulty { get; private set; } = Difficulty.Medium;

    /// <inheritdoc/>
    public BoardConfiguration CurrentConfiguration { get; private set; }

    /// <inheritdoc/>
    public event Func<Task>? GameStateChanged;

    /// <inheritdoc/>
    public void StartGame(GameMode mode, Difficulty difficulty, BoardConfiguration configuration)
    {
        Board = new Board(configuration);
        CurrentConfiguration = configuration;
        CurrentPlayer = Player.X;
        Result = GameResult.InProgress;
        Mode = mode;
        Difficulty = difficulty;
        NotifyGameStateChanged();
    }

    /// <inheritdoc/>
    public void HumanMove(int index)
    {
        if (Result != GameResult.InProgress) return;
        if (Mode == GameMode.CvC) return;
        if (!Board.IsValidMove(index)) return;

        ApplyMove(index);
    }

    /// <inheritdoc/>
    public void TriggerAiMove()
    {
        if (Result != GameResult.InProgress) return;
        if (Mode == GameMode.PvP) return;

        int move = _aiPlayer.ChooseMove(Board, CurrentPlayer, Difficulty);
        ApplyMove(move);
    }

    private void ApplyMove(int index)
    {
        Board.MakeMove(index, CurrentPlayer);
        Result = Board.CheckResult();

        if (Result == GameResult.InProgress)
            CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;

        NotifyGameStateChanged();
    }

    private void NotifyGameStateChanged()
    {
        var handlers = GameStateChanged?.GetInvocationList().OfType<Func<Task>>().ToArray();
        if (handlers is null || handlers.Length == 0)
            return;

        foreach (var handler in handlers)
            _ = InvokeHandlerAsync(handler);
    }

    private async Task InvokeHandlerAsync(Func<Task> handler)
    {
        try
        {
            await handler().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Game state change handler threw an exception.");
        }
    }
}
