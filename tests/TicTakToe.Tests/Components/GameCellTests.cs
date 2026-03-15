using Bunit;
using Microsoft.AspNetCore.Components;
using TicTakToe.App.Components.Game;

namespace TicTakToe.Tests.Components;

public class GameCellTests : BunitContext
{
    [Fact]
    public void Renders_X_Symbol_WhenValueIsX()
    {
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, 0)
            .Add(c => c.Value, Player.X)
            .Add(c => c.IsClickable, false));

        Assert.Contains("X", cut.Find(".cell-mark").TextContent);
    }

    [Fact]
    public void Renders_O_Symbol_WhenValueIsO()
    {
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, 0)
            .Add(c => c.Value, Player.O)
            .Add(c => c.IsClickable, false));

        Assert.Contains("O", cut.Find(".cell-mark").TextContent);
    }

    [Fact]
    public void Renders_Empty_Symbol_WhenValueIsNone()
    {
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, 0)
            .Add(c => c.Value, Player.None)
            .Add(c => c.IsClickable, false));

        Assert.Equal("", cut.Find(".cell-mark").TextContent);
    }

    [Fact]
    public void HasCellXClass_WhenValueIsX()
    {
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, 0)
            .Add(c => c.Value, Player.X)
            .Add(c => c.IsClickable, false));

        Assert.Contains("cell-x", cut.Find("button.cell").ClassList);
    }

    [Fact]
    public void HasCellOClass_WhenValueIsO()
    {
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, 0)
            .Add(c => c.Value, Player.O)
            .Add(c => c.IsClickable, false));

        Assert.Contains("cell-o", cut.Find("button.cell").ClassList);
    }

    [Fact]
    public void HasCellWinClass_WhenIsWinningCellIsTrue()
    {
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, 0)
            .Add(c => c.Value, Player.X)
            .Add(c => c.IsWinningCell, true)
            .Add(c => c.IsClickable, false));

        Assert.Contains("cell-win", cut.Find("button.cell").ClassList);
    }

    [Fact]
    public void HasCellClickableClass_WhenIsClickableIsTrue()
    {
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, 3)
            .Add(c => c.Value, Player.None)
            .Add(c => c.IsClickable, true));

        Assert.Contains("cell-clickable", cut.Find("button.cell").ClassList);
    }

    [Fact]
    public void IsDisabled_WhenIsClickableIsFalse()
    {
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, 0)
            .Add(c => c.Value, Player.None)
            .Add(c => c.IsClickable, false));

        Assert.True(cut.Find("button.cell").HasAttribute("disabled"));
    }

    [Fact]
    public void Click_Fires_OnCellClick_WithCorrectIndex()
    {
        int? received = null;
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, 5)
            .Add(c => c.Value, Player.None)
            .Add(c => c.IsClickable, true)
            .Add(c => c.OnCellClick, EventCallback.Factory.Create<int>(this, (int i) => received = i)));

        cut.Find("button.cell").Click();

        Assert.Equal(5, received);
    }

    // ── Accessibility (aria-label) ───────────────────────────────────────────

    [Theory]
    [InlineData(0, "Row 1, column 1")]
    [InlineData(2, "Row 1, column 3")]
    [InlineData(4, "Row 2, column 2")]
    [InlineData(6, "Row 3, column 1")]
    [InlineData(8, "Row 3, column 3")]
    public void AriaLabel_IncludesCorrectRowAndColumn(int index, string expectedPrefix)
    {
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, index)
            .Add(c => c.Value, Player.None)
            .Add(c => c.IsClickable, false));

        Assert.StartsWith(expectedPrefix, cut.Find("button.cell").GetAttribute("aria-label"));
    }

    [Fact]
    public void AriaLabel_IncludesEmpty_WhenValueIsNone()
    {
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, 0)
            .Add(c => c.Value, Player.None)
            .Add(c => c.IsClickable, false));

        Assert.Contains(", empty", cut.Find("button.cell").GetAttribute("aria-label"));
    }

    [Fact]
    public void AriaLabel_IncludesMarkedX_WhenValueIsX()
    {
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, 0)
            .Add(c => c.Value, Player.X)
            .Add(c => c.IsClickable, false));

        Assert.Contains(", marked X", cut.Find("button.cell").GetAttribute("aria-label"));
    }

    [Fact]
    public void AriaLabel_IncludesMarkedO_WhenValueIsO()
    {
        var cut = Render<GameCell>(p => p
            .Add(c => c.Index, 0)
            .Add(c => c.Value, Player.O)
            .Add(c => c.IsClickable, false));

        Assert.Contains(", marked O", cut.Find("button.cell").GetAttribute("aria-label"));
    }
}
