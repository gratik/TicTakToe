using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using TicTakToe.App.Infrastructure;

namespace TicTakToe.Tests.Infrastructure;

public class LocalStorageStatsServiceTests
{
    private static (LocalStorageStatsService service, Dictionary<string, string> store) CreateService()
    {
        var store = new Dictionary<string, string>();
        var jsMock = new Mock<IJSRuntime>();

        jsMock.Setup(js => js.InvokeAsync<string?>(
                "tttStorage.getItem",
                It.IsAny<object?[]>()))
            .ReturnsAsync((string method, object?[] args) =>
                store.TryGetValue((string)args[0]!, out var v) ? v : null);

        jsMock.Setup(js => js.InvokeAsync<Microsoft.JSInterop.Infrastructure.IJSVoidResult>(
                "tttStorage.setItem",
                It.IsAny<object?[]>()))
            .Callback((string method, object?[] args) =>
                store[(string)args[0]!] = (string)args[1]!)
            .ReturnsAsync(Mock.Of<Microsoft.JSInterop.Infrastructure.IJSVoidResult>());

        jsMock.Setup(js => js.InvokeAsync<Microsoft.JSInterop.Infrastructure.IJSVoidResult>(
                "tttStorage.removeItem",
                It.IsAny<object?[]>()))
            .Callback((string method, object?[] args) =>
                store.Remove((string)args[0]!))
            .ReturnsAsync(Mock.Of<Microsoft.JSInterop.Infrastructure.IJSVoidResult>());

        var logger = Mock.Of<ILogger<LocalStorageStatsService>>();
        return (new LocalStorageStatsService(jsMock.Object, logger), store);
    }

    [Fact]
    public async Task GetStatsAsync_ReturnsEmpty_WhenNoEntry()
    {
        var (service, _) = CreateService();
        var stats = await service.GetStatsAsync(GameMode.PvP);
        Assert.Equal(GameStats.Empty, stats);
    }

    [Fact]
    public async Task IncrementWinAsync_IncrementsWins()
    {
        var (service, _) = CreateService();
        await service.IncrementWinAsync(GameMode.PvP);
        var stats = await service.GetStatsAsync(GameMode.PvP);
        Assert.Equal(1, stats.Wins);
    }

    [Fact]
    public async Task IncrementLossAsync_IncrementsLosses()
    {
        var (service, _) = CreateService();
        await service.IncrementLossAsync(GameMode.PvC);
        var stats = await service.GetStatsAsync(GameMode.PvC);
        Assert.Equal(1, stats.Losses);
    }

    [Fact]
    public async Task IncrementDrawAsync_IncrementsDraw()
    {
        var (service, _) = CreateService();
        await service.IncrementDrawAsync(GameMode.CvC);
        var stats = await service.GetStatsAsync(GameMode.CvC);
        Assert.Equal(1, stats.Draws);
    }

    [Fact]
    public async Task ResetAllAsync_ClearsAllModeStats()
    {
        var (service, _) = CreateService();
        await service.IncrementWinAsync(GameMode.PvP);
        await service.IncrementWinAsync(GameMode.PvC);
        await service.IncrementWinAsync(GameMode.CvC);

        await service.ResetAllAsync();

        foreach (GameMode mode in Enum.GetValues<GameMode>())
        {
            var stats = await service.GetStatsAsync(mode);
            Assert.Equal(GameStats.Empty, stats);
        }
    }

    [Fact]
    public async Task Multiple_Increments_Accumulate()
    {
        var (service, _) = CreateService();
        await service.IncrementWinAsync(GameMode.PvP);
        await service.IncrementWinAsync(GameMode.PvP);
        await service.IncrementLossAsync(GameMode.PvP);

        var stats = await service.GetStatsAsync(GameMode.PvP);
        Assert.Equal(2, stats.Wins);
        Assert.Equal(1, stats.Losses);
    }

    [Fact]
    public async Task GetStatsAsync_ReturnsEmpty_WhenJsonDeserializesToNull()
    {
        var (service, store) = CreateService();
        // "null" is valid JSON that deserialises to null for a reference type
        store["ttt_stats_PvP"] = "null";

        var stats = await service.GetStatsAsync(GameMode.PvP);

        Assert.Equal(GameStats.Empty, stats);
    }

    [Fact]
    public async Task GetStatsAsync_ReturnsEmpty_WhenJsonIsMalformed_AndClearsEntry()
    {
        var (service, store) = CreateService();
        store["ttt_stats_PvP"] = "not valid json {{{";

        var stats = await service.GetStatsAsync(GameMode.PvP);

        Assert.Equal(GameStats.Empty, stats);
        Assert.False(store.ContainsKey("ttt_stats_PvP"), "Corrupted entry should be self-healed (removed).");
    }
}
