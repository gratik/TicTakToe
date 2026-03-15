using TicTakToe.App.Core.Models;

namespace TicTakToe.App.Core.Services.Strategies;

/// <summary>
/// Hard AI strategy — plays optimally using minimax with alpha-beta pruning.
/// The AI can never be beaten when this strategy is used.
/// </summary>
public sealed class MinimaxStrategy : IAiStrategy
{
    /// <inheritdoc/>
    public Difficulty Difficulty => Difficulty.Hard;

    /// <inheritdoc/>
    public int ChooseMove(Board board, Player player)
    {
        var moves = board.GetAvailableMoves();
        if (moves.Count == 0)
            throw new InvalidOperationException("No available moves.");

        int bestScore = int.MinValue;
        int bestMove = moves[0];

        foreach (var index in moves)
        {
            var clone = board.Clone();
            clone.MakeMove(index, player);
            int score = Minimax(clone, player, false, int.MinValue, int.MaxValue, 0);
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = index;
            }
        }

        return bestMove;
    }

    private int Minimax(Board board, Player maximiser, bool isMaximising,
                        int alpha, int beta, int depth)
    {
        var result = board.CheckResult();

        if (result != GameResult.InProgress)
            return Evaluate(result, maximiser, depth);

        var moves = board.GetAvailableMoves();
        var currentPlayer = isMaximising ? maximiser : Opponent(maximiser);

        if (isMaximising)
        {
            int max = int.MinValue;
            foreach (var index in moves)
            {
                var clone = board.Clone();
                clone.MakeMove(index, currentPlayer);
                int score = Minimax(clone, maximiser, false, alpha, beta, depth + 1);
                max = Math.Max(max, score);
                alpha = Math.Max(alpha, score);
                if (beta <= alpha) break;
            }
            return max;
        }
        else
        {
            int min = int.MaxValue;
            foreach (var index in moves)
            {
                var clone = board.Clone();
                clone.MakeMove(index, currentPlayer);
                int score = Minimax(clone, maximiser, true, alpha, beta, depth + 1);
                min = Math.Min(min, score);
                beta = Math.Min(beta, score);
                if (beta <= alpha) break;
            }
            return min;
        }
    }

    private static int Evaluate(GameResult result, Player maximiser, int depth)
    {
        if (result == GameResult.Draw) return 0;

        bool maximiserWon = (result == GameResult.XWins && maximiser == Player.X)
                         || (result == GameResult.OWins && maximiser == Player.O);

        // Prefer faster wins and slower losses
        return maximiserWon ? 10 - depth : depth - 10;
    }

    private static Player Opponent(Player player) =>
        player == Player.X ? Player.O : Player.X;
}
