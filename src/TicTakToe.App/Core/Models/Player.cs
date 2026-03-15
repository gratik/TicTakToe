namespace TicTakToe.App.Core.Models;

/// <summary>Identifies whose turn it is or who won.</summary>
public enum Player
{
    /// <summary>No player — empty cell.</summary>
    None = 0,
    /// <summary>Player X.</summary>
    X = 1,
    /// <summary>Player O.</summary>
    O = 2
}
