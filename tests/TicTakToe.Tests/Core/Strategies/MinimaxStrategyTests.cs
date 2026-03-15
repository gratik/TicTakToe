namespace TicTakToe.Tests.Core.Strategies;

public class MinimaxStrategyTests
{
    private readonly MinimaxStrategy _strategy = new();

    [Fact]
    public void Difficulty_IsHard()
    {
        Assert.Equal(Difficulty.Hard, _strategy.Difficulty);
    }

    [Fact]
    public void ChooseMove_WinsImmediately_WhenWinAvailable()
    {
        var board = new Board();
        board.MakeMove(0, Player.X);
        board.MakeMove(1, Player.X);
        // Index 2 wins
        int move = _strategy.ChooseMove(board, Player.X);
        Assert.Equal(2, move);
    }

    [Fact]
    public void ChooseMove_BlocksOpponentWin()
    {
        var board = new Board();
        board.MakeMove(0, Player.O);
        board.MakeMove(1, Player.O);
        // X must block at 2
        int move = _strategy.ChooseMove(board, Player.X);
        Assert.Equal(2, move);
    }

    /// <summary>
    /// Hard AI should never lose against any human strategy.
    /// Exhaustively plays X (minimax) vs O (every possible human response).
    /// </summary>
    [Fact]
    public void HardAi_NeverLoses_AsX_AgainstAnyOpponent()
    {
        PlayAllGames(Player.X, _strategy);
    }

    [Fact]
    public void HardAi_NeverLoses_AsO_AgainstAnyOpponent()
    {
        PlayAllGames(Player.O, _strategy);
    }

    private static void PlayAllGames(Player aiPlayer, MinimaxStrategy strategy)
    {
        var opponent = aiPlayer == Player.X ? Player.O : Player.X;
        var results = new List<GameResult>();
        Simulate(new Board(), Player.X, aiPlayer, strategy, results);

        foreach (var result in results)
        {
            // AI should not lose
            if (aiPlayer == Player.X)
                Assert.NotEqual(GameResult.OWins, result);
            else
                Assert.NotEqual(GameResult.XWins, result);
        }
    }

    private static void Simulate(Board board, Player current, Player aiPlayer,
                                  MinimaxStrategy strategy, List<GameResult> outcomes)
    {
        var result = board.CheckResult();
        if (result != GameResult.InProgress)
        {
            outcomes.Add(result);
            return;
        }

        if (current == aiPlayer)
        {
            var move = strategy.ChooseMove(board, current);
            var clone = board.Clone();
            clone.MakeMove(move, current);
            var next = current == Player.X ? Player.O : Player.X;
            Simulate(clone, next, aiPlayer, strategy, outcomes);
        }
        else
        {
            foreach (var move in board.GetAvailableMoves())
            {
                var clone = board.Clone();
                clone.MakeMove(move, current);
                var next = current == Player.X ? Player.O : Player.X;
                Simulate(clone, next, aiPlayer, strategy, outcomes);
            }
        }
    }
}
