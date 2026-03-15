namespace TicTakToe.Tests.Core.Strategies;

public class WeightedStrategyTests
{
    private readonly WeightedStrategy _strategy = new();

    [Fact]
    public void Difficulty_IsMedium()
    {
        Assert.Equal(Difficulty.Medium, _strategy.Difficulty);
    }

    [Fact]
    public void ChooseMove_WinsImmediately_WhenWinAvailable()
    {
        var board = new Board();
        board.MakeMove(0, Player.X);
        board.MakeMove(1, Player.X);
        // Index 2 completes top row for X

        int move = _strategy.ChooseMove(board, Player.X);
        Assert.Equal(2, move);
    }

    [Fact]
    public void ChooseMove_BlocksOpponent_WhenOpponentAboutToWin()
    {
        var board = new Board();
        board.MakeMove(0, Player.O);
        board.MakeMove(1, Player.O);
        // Index 2 would complete top row for O — X should block

        int move = _strategy.ChooseMove(board, Player.X);
        Assert.Equal(2, move);
    }

    [Fact]
    public void ChooseMove_PrefersWinOverBlock_WhenBothAvailable()
    {
        var board = new Board();
        board.MakeMove(0, Player.X);
        board.MakeMove(1, Player.X);
        // X can win at 2
        board.MakeMove(6, Player.O);
        board.MakeMove(7, Player.O);
        // O can win at 8

        int move = _strategy.ChooseMove(board, Player.X);
        Assert.Equal(2, move); // should win, not block
    }
}
