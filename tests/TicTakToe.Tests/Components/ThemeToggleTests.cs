using Bunit;
using Microsoft.AspNetCore.Components;
using TicTakToe.App.Components.Layout;

namespace TicTakToe.Tests.Components;

public class ThemeToggleTests : BunitContext
{
    [Fact]
    public void Renders_Button_WithThemeToggleClass()
    {
        var cut = Render<ThemeToggle>();

        Assert.NotNull(cut.Find("button.theme-toggle"));
    }

    [Fact]
    public void Button_HasAriaLabel()
    {
        var cut = Render<ThemeToggle>();

        Assert.Equal("Toggle theme", cut.Find("button.theme-toggle").GetAttribute("aria-label"));
    }

    [Fact]
    public void Renders_BothThemeIcons()
    {
        var cut = Render<ThemeToggle>();

        Assert.NotNull(cut.Find(".theme-icon-light"));
        Assert.NotNull(cut.Find(".theme-icon-dark"));
    }

    [Fact]
    public void AcceptsThemeParameter()
    {
        // Verifies the component renders without error when Theme is set
        var cut = Render<ThemeToggle>(p => p
            .Add(t => t.Theme, "dark"));

        Assert.NotNull(cut.Find("button.theme-toggle"));
    }

    [Fact]
    public void OnToggle_Callback_IsOptional()
    {
        // Should render without providing OnToggle
        var cut = Render<ThemeToggle>(p => p
            .Add(t => t.Theme, "light"));

        Assert.NotNull(cut.Find("button.theme-toggle"));
    }
}
