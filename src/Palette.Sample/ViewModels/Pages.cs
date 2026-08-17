namespace Palette.Sample.ViewModels;

public sealed class OverviewViewModel : PageViewModel
{
    public override string Title => "Overview";
    public override string Glyph => "◆";
    public override string Blurb => "What this template gives you, and how the live palette engine works.";
}

public sealed class LayoutViewModel : PageViewModel
{
    public override string Title => "Layout & pages";
    public override string Glyph => "▦";
    public override string Blurb => "Surfaces, cards, wells and typography — the building blocks of a page.";
}

public sealed class EditorViewModel : PageViewModel
{
    public override string Title => "Editor surface";
    public override string Glyph => "⌨";
    public override string Blurb => "The view you look at 90% of the day: text area, syntax-highlighted file, gutter.";
}

public sealed class DiffViewModel : PageViewModel
{
    public override string Title => "Git diff";
    public override string Glyph => "±";
    public override string Blurb => "Added / removed / hunk colours tuned for scanning changes without glare.";
}

public sealed class ControlsViewModel : PageViewModel
{
    public override string Title => "Buttons & alerts";
    public override string Glyph => "⬡";
    public override string Blurb => "Accent, neutral and danger actions, status text and alert banners.";
}
