using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using TicTakToe.App.Core.Models;
using TicTakToe.App.Core.Services.Interfaces;

namespace TicTakToe.App.Infrastructure;

/// <summary>
/// Persists game statistics in the browser's <c>localStorage</c> via JS interop.
/// Stats are stored per <see cref="GameMode"/> as JSON under keys prefixed with <c>ttt_stats_</c>.
/// </summary>
public sealed class LocalStorageStatsService : IStatsService
{
    private readonly IJSRuntime _js;
    private readonly ILogger<LocalStorageStatsService> _logger;
    private readonly Dictionary<GameMode, GameStats> _cache = new();

    /// <summary>Initialises a new instance backed by the provided <see cref="IJSRuntime"/>.</summary>
    public LocalStorageStatsService(IJSRuntime js, ILogger<LocalStorageStatsService> logger)
    {
        _js = js;
        _logger = logger;
        foreach (GameMode mode in Enum.GetValues<GameMode>())
            _cache[mode] = GameStats.Empty;
    }

    /// <inheritdoc/>
    public async Task<GameStats> GetStatsAsync(GameMode mode)
    {
        try
        {
            var json = await _js.InvokeAsync<string?>("tttStorage.getItem", Key(mode));
            if (string.IsNullOrEmpty(json))
                return _cache[mode];

            try
            {
                var stats = JsonSerializer.Deserialize<GameStats>(json) ?? GameStats.Empty;
                _cache[mode] = stats;
                return stats;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Corrupted stats payload for {Mode}; resetting stored value.", mode);
                await SafeRemoveAsync(mode);
                _cache[mode] = GameStats.Empty;
                return GameStats.Empty;
            }
        }
        catch (JSException ex)
        {
            _logger.LogWarning(ex, "Unable to read stats from localStorage for {Mode}; using in-memory cache.", mode);
            return _cache[mode];
        }
    }

    /// <inheritdoc/>
    public async Task IncrementWinAsync(GameMode mode)
    {
        var stats = (await GetStatsAsync(mode)).WithWin();
        await SaveAsync(mode, stats);
    }

    /// <inheritdoc/>
    public async Task IncrementLossAsync(GameMode mode)
    {
        var stats = (await GetStatsAsync(mode)).WithLoss();
        await SaveAsync(mode, stats);
    }

    /// <inheritdoc/>
    public async Task IncrementDrawAsync(GameMode mode)
    {
        var stats = (await GetStatsAsync(mode)).WithDraw();
        await SaveAsync(mode, stats);
    }

    /// <inheritdoc/>
    public async Task ResetAllAsync()
    {
        foreach (GameMode mode in Enum.GetValues<GameMode>())
        {
            _cache[mode] = GameStats.Empty;
            await SafeRemoveAsync(mode);
        }
    }

    private async Task SaveAsync(GameMode mode, GameStats stats)
    {
        _cache[mode] = stats;
        var json = JsonSerializer.Serialize(stats);
        try
        {
            await _js.InvokeVoidAsync("tttStorage.setItem", Key(mode), json);
        }
        catch (JSException ex)
        {
            _logger.LogWarning(ex, "Unable to persist stats for {Mode}; keeping in-memory cache.", mode);
        }
    }

    private static string Key(GameMode mode) => $"ttt_stats_{mode}";

    private async Task SafeRemoveAsync(GameMode mode)
    {
        try
        {
            await _js.InvokeVoidAsync("tttStorage.removeItem", Key(mode));
        }
        catch (JSException ex)
        {
            _logger.LogWarning(ex, "Unable to remove stored stats for {Mode}.", mode);
        }
    }
}
