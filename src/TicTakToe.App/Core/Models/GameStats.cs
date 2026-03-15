namespace TicTakToe.App.Core.Models;

/// <summary>Immutable snapshot of win/loss/draw statistics for a given game mode.</summary>
/// <param name="Wins">Number of games won.</param>
/// <param name="Losses">Number of games lost.</param>
/// <param name="Draws">Number of games drawn.</param>
public record GameStats(int Wins, int Losses, int Draws)
{
    /// <summary>Total number of games played.</summary>
    public int Total => Wins + Losses + Draws;

    /// <summary>Returns a new <see cref="GameStats"/> with the win count incremented by one.</summary>
    public GameStats WithWin() => this with { Wins = Wins + 1 };

    /// <summary>Returns a new <see cref="GameStats"/> with the loss count incremented by one.</summary>
    public GameStats WithLoss() => this with { Losses = Losses + 1 };

    /// <summary>Returns a new <see cref="GameStats"/> with the draw count incremented by one.</summary>
    public GameStats WithDraw() => this with { Draws = Draws + 1 };

    /// <summary>A zeroed-out stats instance.</summary>
    public static readonly GameStats Empty = new(0, 0, 0);
}
