using TicTakToe.App.Core.Models;

namespace TicTakToe.App.Core.Services.Interfaces;

/// <summary>Manages the lifecycle and state of a single Tic-Tac-Toe game.</summary>
public interface IGameEngine
{
    /// <summary>The current board state.</summary>
    Board Board { get; }

    /// <summary>The player whose turn it currently is.</summary>
    Player CurrentPlayer { get; }

    /// <summary>The result of the game (or <see cref="GameResult.InProgress"/>).</summary>
    GameResult Result { get; }

    /// <summary>The active game mode.</summary>
    GameMode Mode { get; }

    /// <summary>The active AI difficulty.</summary>
    Difficulty Difficulty { get; }

    /// <summary>Raised whenever game state changes (move made, game over, new game).</summary>
    event Action? GameStateChanged;

    /// <summary>Starts a fresh game with the specified mode and difficulty.</summary>
    void StartGame(GameMode mode, Difficulty difficulty);

    /// <summary>
    /// Applies a human move at <paramref name="index"/>.
    /// No-ops if the game is over or the cell is invalid.
    /// </summary>
    void HumanMove(int index);

    /// <summary>
    /// Triggers the AI to make its move for the current player.
    /// No-ops if the game is over.
    /// </summary>
    void TriggerAiMove();
}
