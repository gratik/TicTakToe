using TicTakToe.App.Core.Models;
using TicTakToe.App.Core.Services.Interfaces;

namespace TicTakToe.App.Core.Services;

/// <summary>
/// Orchestrates turns, enforces rules, and raises state-change events
/// for a single Tic-Tac-Toe game session.
/// </summary>
public sealed class GameEngine : IGameEngine
{
    private readonly IAiPlayer _aiPlayer;

    /// <summary>Initialises a new <see cref="GameEngine"/> with the supplied AI player.</summary>
    public GameEngine(IAiPlayer aiPlayer)
    {
        _aiPlayer = aiPlayer;
        Board = new Board();
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
    public event Action? GameStateChanged;

    /// <inheritdoc/>
    public void StartGame(GameMode mode, Difficulty difficulty)
    {
        Board = new Board();
        CurrentPlayer = Player.X;
        Result = GameResult.InProgress;
        Mode = mode;
        Difficulty = difficulty;
        GameStateChanged?.Invoke();
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

        GameStateChanged?.Invoke();
    }
}
