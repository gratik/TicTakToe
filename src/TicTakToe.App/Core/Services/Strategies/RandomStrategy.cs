using TicTakToe.App.Core.Models;

namespace TicTakToe.App.Core.Services.Strategies;

/// <summary>
/// Easy AI strategy — selects a random available cell.
/// </summary>
public sealed class RandomStrategy : IAiStrategy
{
    /// <summary>
    /// Returns all available moves with null score for visualization.
    /// </summary>
    public IReadOnlyList<AiMoveEvaluation> GetMoveEvaluations(Board board, Player player)
    {
        var moves = board.GetAvailableMoves();
        return moves.Select(i => new AiMoveEvaluation(i, null, null)).ToList();
    }
    private readonly Random _random;

    /// <summary>Initialises a new instance with an optional <see cref="Random"/> source.</summary>
    public RandomStrategy(Random? random = null) => _random = random ?? Random.Shared;

    /// <inheritdoc/>
    public Difficulty Difficulty => Difficulty.Easy;

    /// <inheritdoc/>
    public int ChooseMove(Board board, Player player)
    {
        var moves = board.GetAvailableMoves();
        if (moves.Count == 0)
            throw new InvalidOperationException("No available moves.");

        return moves[_random.Next(moves.Count)];
    }
}
