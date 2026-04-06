namespace TicTakToe.App.Core.Models;

/// <summary>
/// Represents an AI-evaluated move and its score (if available).
/// </summary>
public record AiMoveEvaluation(int Index, int? Score, string? Label = null);