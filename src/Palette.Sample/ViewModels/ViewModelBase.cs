using CommunityToolkit.Mvvm.ComponentModel;

namespace Palette.Sample.ViewModels;

public abstract class ViewModelBase : ObservableObject;

/// <summary>A navigable page shown in the content host. Matched by <see cref="ViewLocator"/>.</summary>
public abstract partial class PageViewModel : ViewModelBase
{
    /// <summary>Title shown in the nav and page header.</summary>
    public abstract string Title { get; }

    /// <summary>One-line description shown under the page title.</summary>
    public virtual string Blurb => "";

    /// <summary>A leading glyph for the nav row (emoji/text — keeps the demo font-independent).</summary>
    public virtual string Glyph => "•";

    /// <summary>True while this is the selected page (drives nav highlight).</summary>
    [ObservableProperty]
    private bool _isActive;
}

/// <summary>A non-clickable section label in the nav rail.</summary>
public sealed class NavHeaderViewModel(string label)
{
    public string Label { get; } = label;
}
