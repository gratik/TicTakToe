using TicTakToe.App.Core.Models;

namespace TicTakToe.App.Core.Services.Interfaces;

/// <summary>Selects the best move for an AI player given a board state and difficulty.</summary>
public interface IAiPlayer
{
    /// <summary>
    /// Chooses the cell index the AI should play.
    /// </summary>
    /// <param name="board">Current board state (not mutated).</param>
    /// <param name="player">The AI player making the move.</param>
    /// <param name="difficulty">Desired difficulty level.</param>
    /// <returns>A valid cell index (0–8).</returns>
    int ChooseMove(Board board, Player player, Difficulty difficulty);

    /// <summary>
    /// Returns all moves considered by the AI and their scores/labels for visualization overlays.
    /// </summary>
    IReadOnlyList<AiMoveEvaluation> GetMoveEvaluations(Board board, Player player, Difficulty difficulty);
}
