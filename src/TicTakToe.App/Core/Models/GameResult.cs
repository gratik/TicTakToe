namespace TicTakToe.App.Core.Models;

/// <summary>Represents the outcome of a completed or in-progress game.</summary>
public enum GameResult
{
    /// <summary>The game is still in progress.</summary>
    InProgress,
    /// <summary>Player X has won.</summary>
    XWins,
    /// <summary>Player O has won.</summary>
    OWins,
    /// <summary>The game ended in a draw.</summary>
    Draw
}
