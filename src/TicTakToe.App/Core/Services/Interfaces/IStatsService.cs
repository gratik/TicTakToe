using TicTakToe.App.Core.Models;

namespace TicTakToe.App.Core.Services.Interfaces;

/// <summary>Provides persistence of win/loss/draw statistics per game mode.</summary>
public interface IStatsService
{
    /// <summary>Retrieves current stats for the specified game mode.</summary>
    Task<GameStats> GetStatsAsync(GameMode mode);

    /// <summary>Increments the win counter for the specified game mode.</summary>
    Task IncrementWinAsync(GameMode mode);

    /// <summary>Increments the loss counter for the specified game mode.</summary>
    Task IncrementLossAsync(GameMode mode);

    /// <summary>Increments the draw counter for the specified game mode.</summary>
    Task IncrementDrawAsync(GameMode mode);

    /// <summary>Resets all stats for all game modes to zero.</summary>
    Task ResetAllAsync();
}
