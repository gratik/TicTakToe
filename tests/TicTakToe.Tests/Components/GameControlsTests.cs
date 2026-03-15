using Bunit;
using Microsoft.AspNetCore.Components;
using TicTakToe.App.Components.Game;

namespace TicTakToe.Tests.Components;

public class GameControlsTests : BunitContext
{
    [Fact]
    public void Renders_ModeSelect_WithAllThreeModes()
    {
        var cut = Render<GameControls>(p => p
            .Add(c => c.Mode, GameMode.PvP)
            .Add(c => c.Difficulty, Difficulty.Medium));

        var options = cut.FindAll("#mode-select option");
        Assert.Equal(3, options.Count);
    }

    [Fact]
    public void Renders_DifficultySelect_WithAllThreeLevels()
    {
        var cut = Render<GameControls>(p => p
            .Add(c => c.Mode, GameMode.PvP)
            .Add(c => c.Difficulty, Difficulty.Medium));

        var options = cut.FindAll("#diff-select option");
        Assert.Equal(3, options.Count);
    }

    [Fact]
    public void DifficultySelect_IsDisabled_InPvPMode()
    {
        var cut = Render<GameControls>(p => p
            .Add(c => c.Mode, GameMode.PvP)
            .Add(c => c.Difficulty, Difficulty.Medium));

        Assert.True(cut.Find("#diff-select").HasAttribute("disabled"));
    }

    [Fact]
    public void DifficultySelect_IsEnabled_InPvCMode()
    {
        var cut = Render<GameControls>(p => p
            .Add(c => c.Mode, GameMode.PvC)
            .Add(c => c.Difficulty, Difficulty.Medium));

        Assert.False(cut.Find("#diff-select").HasAttribute("disabled"));
    }

    [Fact]
    public void DifficultySelect_IsEnabled_InCvCMode()
    {
        var cut = Render<GameControls>(p => p
            .Add(c => c.Mode, GameMode.CvC)
            .Add(c => c.Difficulty, Difficulty.Hard));

        Assert.False(cut.Find("#diff-select").HasAttribute("disabled"));
    }

    [Fact]
    public void NewGame_Button_Fires_OnNewGame()
    {
        bool fired = false;
        var cut = Render<GameControls>(p => p
            .Add(c => c.Mode, GameMode.PvP)
            .Add(c => c.Difficulty, Difficulty.Medium)
            .Add(c => c.OnNewGame, EventCallback.Factory.Create(this, () => fired = true)));

        cut.Find("button.btn-primary").Click();

        Assert.True(fired);
    }

    [Fact]
    public void ModeSelect_Change_Fires_OnModeChanged_WithCorrectValue()
    {
        GameMode? received = null;
        var cut = Render<GameControls>(p => p
            .Add(c => c.Mode, GameMode.PvP)
            .Add(c => c.Difficulty, Difficulty.Medium)
            .Add(c => c.OnModeChanged, EventCallback.Factory.Create<GameMode>(this, (GameMode m) => received = m)));

        cut.Find("#mode-select").Change("PvC");

        Assert.Equal(GameMode.PvC, received);
    }

    [Fact]
    public void ModeSelect_Change_ToCvC_Fires_OnModeChanged()
    {
        GameMode? received = null;
        var cut = Render<GameControls>(p => p
            .Add(c => c.Mode, GameMode.PvP)
            .Add(c => c.Difficulty, Difficulty.Medium)
            .Add(c => c.OnModeChanged, EventCallback.Factory.Create<GameMode>(this, (GameMode m) => received = m)));

        cut.Find("#mode-select").Change("CvC");

        Assert.Equal(GameMode.CvC, received);
    }

    [Fact]
    public void DifficultySelect_Change_Fires_OnDifficultyChanged_WithCorrectValue()
    {
        Difficulty? received = null;
        var cut = Render<GameControls>(p => p
            .Add(c => c.Mode, GameMode.PvC)
            .Add(c => c.Difficulty, Difficulty.Easy)
            .Add(c => c.OnDifficultyChanged, EventCallback.Factory.Create<Difficulty>(this, (Difficulty d) => received = d)));

        cut.Find("#diff-select").Change("Hard");

        Assert.Equal(Difficulty.Hard, received);
    }

    [Fact]
    public void DifficultySelect_Change_ToMedium_Fires_OnDifficultyChanged()
    {
        Difficulty? received = null;
        var cut = Render<GameControls>(p => p
            .Add(c => c.Mode, GameMode.PvC)
            .Add(c => c.Difficulty, Difficulty.Easy)
            .Add(c => c.OnDifficultyChanged, EventCallback.Factory.Create<Difficulty>(this, (Difficulty d) => received = d)));

        cut.Find("#diff-select").Change("Medium");

        Assert.Equal(Difficulty.Medium, received);
    }
}
