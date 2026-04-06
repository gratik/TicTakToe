using TicTakToe.App.Core.Models;

namespace TicTakToe.App.Core.Services.Strategies;

/// <summary>
/// Medium AI strategy — wins immediately if possible, blocks opponent wins,
/// otherwise falls back to a random move.
/// </summary>
public sealed class WeightedStrategy : IAiStrategy
{
    /// <summary>
    /// Returns all available moves, labeling win/block/other for visualization.
    /// </summary>
    public IReadOnlyList<AiMoveEvaluation> GetMoveEvaluations(Board board, Player player)
    {
        var opponent = player == Player.X ? Player.O : Player.X;
        var moves = board.GetAvailableMoves();
        var evaluations = new List<AiMoveEvaluation>();
        foreach (var index in moves)
        {
            var cloneWin = board.Clone();
            cloneWin.MakeMove(index, player);
            var win = cloneWin.CheckResult();
            if ((win == GameResult.XWins && player == Player.X) || (win == GameResult.OWins && player == Player.O))
            {
                evaluations.Add(new AiMoveEvaluation(index, 2, "Win"));
                continue;
            }
            var cloneBlock = board.Clone();
            cloneBlock.MakeMove(index, opponent);
            var block = cloneBlock.CheckResult();
            if ((block == GameResult.XWins && opponent == Player.X) || (block == GameResult.OWins && opponent == Player.O))
            {
                evaluations.Add(new AiMoveEvaluation(index, 1, "Block"));
                continue;
            }
            evaluations.Add(new AiMoveEvaluation(index, 0, null));
        }
        return evaluations;
    }
    private readonly Random _random;

    /// <summary>Initialises a new instance with an optional <see cref="Random"/> source.</summary>
    public WeightedStrategy(Random? random = null) => _random = random ?? Random.Shared;

    /// <inheritdoc/>
    public Difficulty Difficulty => Difficulty.Medium;

    /// <inheritdoc/>
    public int ChooseMove(Board board, Player player)
    {
        var opponent = player == Player.X ? Player.O : Player.X;

        // 1. Win immediately
        var win = FindWinningMove(board, player);
        if (win.HasValue) return win.Value;

        // 2. Block opponent win
        var block = FindWinningMove(board, opponent);
        if (block.HasValue) return block.Value;

        // 3. Random fallback
        var moves = board.GetAvailableMoves();
        return moves[_random.Next(moves.Count)];
    }

    private static int? FindWinningMove(Board board, Player player)
    {
        foreach (var index in board.GetAvailableMoves())
        {
            var clone = board.Clone();
            clone.MakeMove(index, player);
            var result = clone.CheckResult();
            if (result == GameResult.XWins && player == Player.X) return index;
            if (result == GameResult.OWins && player == Player.O) return index;
        }
        return null;
    }
}
