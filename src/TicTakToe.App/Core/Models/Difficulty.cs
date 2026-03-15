namespace TicTakToe.App.Core.Models;

/// <summary>AI difficulty level controlling which strategy the computer uses.</summary>
public enum Difficulty
{
    /// <summary>Computer plays randomly.</summary>
    Easy,
    /// <summary>Computer wins or blocks when possible, otherwise plays randomly.</summary>
    Medium,
    /// <summary>Computer plays optimally using minimax with alpha-beta pruning.</summary>
    Hard
}
