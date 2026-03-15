namespace TicTakToe.Tests.Core;

public class AiPlayerTests
{
    private static AiPlayer BuildAiPlayer() =>
        new([new RandomStrategy(), new WeightedStrategy(), new MinimaxStrategy()]);

    [Theory]
    [InlineData(Difficulty.Easy)]
    [InlineData(Difficulty.Medium)]
    [InlineData(Difficulty.Hard)]
    public void ChooseMove_ReturnsValidIndex_ForAllDifficulties(Difficulty difficulty)
    {
        var ai = BuildAiPlayer();
        var board = new Board();
        int move = ai.ChooseMove(board, Player.X, difficulty);
        Assert.InRange(move, 0, 8);
    }

    [Fact]
    public void Constructor_Throws_WhenStrategyMissing()
    {
        // Only provide Easy — Medium and Hard are missing
        Assert.Throws<ArgumentException>(() => new AiPlayer([new RandomStrategy()]));
    }

    [Fact]
    public void ChooseMove_Throws_ForUnknownDifficulty()
    {
        var ai = BuildAiPlayer();
        var board = new Board();
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ai.ChooseMove(board, Player.X, (Difficulty)99));
    }
}
