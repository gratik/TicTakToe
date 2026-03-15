using System.Text.Json;
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

    /// <summary>Initialises a new instance backed by the provided <see cref="IJSRuntime"/>.</summary>
    public LocalStorageStatsService(IJSRuntime js) => _js = js;

    /// <inheritdoc/>
    public async Task<GameStats> GetStatsAsync(GameMode mode)
    {
        var json = await _js.InvokeAsync<string?>("tttStorage.getItem", Key(mode));
        if (string.IsNullOrEmpty(json)) return GameStats.Empty;

        try
        {
            return JsonSerializer.Deserialize<GameStats>(json) ?? GameStats.Empty;
        }
        catch (JsonException)
        {
            // Self-heal: remove the corrupted entry so it doesn't fail on every load.
            await _js.InvokeVoidAsync("tttStorage.removeItem", Key(mode));
            return GameStats.Empty;
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
            await _js.InvokeVoidAsync("tttStorage.removeItem", Key(mode));
    }

    private async Task SaveAsync(GameMode mode, GameStats stats)
    {
        var json = JsonSerializer.Serialize(stats);
        await _js.InvokeVoidAsync("tttStorage.setItem", Key(mode), json);
    }

    private static string Key(GameMode mode) => $"ttt_stats_{mode}";
}
