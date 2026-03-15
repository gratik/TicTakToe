namespace TicTakToe.Tests.Core.Strategies;

public class RandomStrategyTests
{
    [Fact]
    public void ChooseMove_ReturnsValidIndex_WhenMovesAvailable()
    {
        var strategy = new RandomStrategy();
        var board = new Board();
        int move = strategy.ChooseMove(board, Player.X);
        Assert.InRange(move, 0, 8);
    }

    [Fact]
    public void ChooseMove_ReturnsOnlyAvailableCell_WhenOneMoveLeft()
    {
        var board = new Board();
        // Fill all except index 7
        for (int i = 0; i < 9; i++)
            if (i != 7) board.MakeMove(i, i % 2 == 0 ? Player.X : Player.O);

        var strategy = new RandomStrategy();
        int move = strategy.ChooseMove(board, Player.X);
        Assert.Equal(7, move);
    }

    [Fact]
    public void ChooseMove_Throws_WhenNoMovesAvailable()
    {
        var board = new Board();
        // Fill board with a draw position
        int[] xMoves = [0, 2, 4, 7];
        int[] oMoves = [1, 3, 5, 6, 8];
        foreach (var i in xMoves) board.MakeMove(i, Player.X);
        foreach (var i in oMoves) board.MakeMove(i, Player.O);

        var strategy = new RandomStrategy();
        Assert.Throws<InvalidOperationException>(() => strategy.ChooseMove(board, Player.X));
    }

    [Fact]
    public void Difficulty_IsEasy()
    {
        Assert.Equal(Difficulty.Easy, new RandomStrategy().Difficulty);
    }
}
