using TicTakToe.App.Core.Models;

namespace TicTakToe.App.Core.Services.Strategies;

/// <summary>
/// Defines a single AI move-selection strategy.
/// Implementations vary by difficulty level.
/// </summary>
public interface IAiStrategy
{
    /// <summary>Gets the difficulty level this strategy handles.</summary>
    Difficulty Difficulty { get; }

    /// <summary>
    /// Chooses and returns the best cell index for <paramref name="player"/> given the current <paramref name="board"/>.
    /// </summary>
    /// <param name="board">Current board state (not mutated).</param>
    /// <param name="player">The AI player making the move.</param>
    /// <returns>A valid cell index (0–8).</returns>
    int ChooseMove(Board board, Player player);

    /// <summary>
    /// Returns a list of all moves considered by the AI and their evaluation scores (if available).
    /// For Minimax: all root-level moves and their minimax scores.
    /// For Weighted: moves checked for win/block (score 2=win, 1=block, 0=other).
    /// For Random: all available moves (score null).
    /// </summary>
    IReadOnlyList<AiMoveEvaluation> GetMoveEvaluations(Board board, Player player);
}
