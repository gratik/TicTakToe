namespace TicTakToe.App.Core.Models;

/// <summary>Defines how the two sides of a game are controlled.</summary>
public enum GameMode
{
    /// <summary>Two human players on the same screen.</summary>
    PvP,
    /// <summary>One human player (X) against the computer (O).</summary>
    PvC,
    /// <summary>Both sides are controlled by the computer.</summary>
    CvC
}
