namespace TicTakToe.Tests.Core;

public class BoardTests
{
    [Fact]
    public void NewBoard_AllCellsEmpty()
    {
        var board = new Board();
        Assert.All(board.Cells, c => Assert.Equal(Player.None, c));
    }

    [Fact]
    public void IsValidMove_ReturnsFalse_ForOccupiedCell()
    {
        var board = new Board();
        board.MakeMove(0, Player.X);
        Assert.False(board.IsValidMove(0));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    public void IsValidMove_ReturnsFalse_ForOutOfRangeIndex(int index)
    {
        var board = new Board();
        Assert.False(board.IsValidMove(index));
    }

    [Fact]
    public void MakeMove_PlacesPlayer_OnEmptyCell()
    {
        var board = new Board();
        board.MakeMove(4, Player.X);
        Assert.Equal(Player.X, board[4]);
    }

    [Fact]
    public void MakeMove_Throws_OnOccupiedCell()
    {
        var board = new Board();
        board.MakeMove(0, Player.X);
        Assert.Throws<InvalidOperationException>(() => board.MakeMove(0, Player.O));
    }

    [Theory]
    [InlineData(0, 1, 2)] // top row
    [InlineData(3, 4, 5)] // mid row
    [InlineData(6, 7, 8)] // bottom row
    [InlineData(0, 3, 6)] // left col
    [InlineData(1, 4, 7)] // mid col
    [InlineData(2, 5, 8)] // right col
    [InlineData(0, 4, 8)] // diag
    [InlineData(2, 4, 6)] // anti-diag
    public void CheckResult_DetectsXWin_ForAllWinLines(int a, int b, int c)
    {
        var board = new Board();
        board.MakeMove(a, Player.X);
        board.MakeMove(b, Player.X);
        board.MakeMove(c, Player.X);
        Assert.Equal(GameResult.XWins, board.CheckResult());
    }

    [Fact]
    public void CheckResult_DetectsOWin()
    {
        var board = new Board();
        board.MakeMove(0, Player.O);
        board.MakeMove(1, Player.O);
        board.MakeMove(2, Player.O);
        Assert.Equal(GameResult.OWins, board.CheckResult());
    }

    [Fact]
    public void CheckResult_ReturnsDraw_WhenBoardFull_NoWinner()
    {
        var board = new Board();
        // X O X / O X O / O X O — draw
        int[] xMoves = [0, 2, 4, 7];
        int[] oMoves = [1, 3, 5, 6, 8];
        foreach (var i in xMoves) board.MakeMove(i, Player.X);
        foreach (var i in oMoves) board.MakeMove(i, Player.O);
        Assert.Equal(GameResult.Draw, board.CheckResult());
    }

    [Fact]
    public void CheckResult_ReturnsInProgress_WhenMovesRemain()
    {
        var board = new Board();
        board.MakeMove(0, Player.X);
        Assert.Equal(GameResult.InProgress, board.CheckResult());
    }

    [Fact]
    public void GetAvailableMoves_ReturnsAllNine_WhenEmpty()
    {
        var board = new Board();
        Assert.Equal(9, board.GetAvailableMoves().Count);
    }

    [Fact]
    public void GetAvailableMoves_ExcludesOccupiedCells()
    {
        var board = new Board();
        board.MakeMove(0, Player.X);
        board.MakeMove(4, Player.O);
        var moves = board.GetAvailableMoves();
        Assert.Equal(7, moves.Count);
        Assert.DoesNotContain(0, moves);
        Assert.DoesNotContain(4, moves);
    }

    [Fact]
    public void Clone_CreatesIndependentCopy()
    {
        var board = new Board();
        board.MakeMove(0, Player.X);
        var clone = board.Clone();
        clone.MakeMove(1, Player.O);

        Assert.Equal(Player.None, board[1]); // original unaffected
    }

    [Fact]
    public void GetWinningLine_ReturnsNull_WhenNoWinner()
    {
        var board = new Board();
        Assert.Null(board.GetWinningLine());
    }

    [Fact]
    public void GetWinningLine_ReturnsCorrectLine()
    {
        var board = new Board();
        board.MakeMove(0, Player.X);
        board.MakeMove(1, Player.X);
        board.MakeMove(2, Player.X);
        var line = board.GetWinningLine();
        Assert.NotNull(line);
        Assert.Equal([0, 1, 2], line);
    }
}
