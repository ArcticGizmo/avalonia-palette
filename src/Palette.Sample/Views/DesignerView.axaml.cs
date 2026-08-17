using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Palette.Sample.ViewModels;

namespace Palette.Sample.Views;

public partial class DesignerView : UserControl
{
    public DesignerView()
    {
        InitializeComponent();
        ExportButton.Click += OnExport;
        ImportButton.Click += OnImport;
    }

    private async void OnExport(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DesignerViewModel vm) return;
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null) return;
        await clipboard.SetTextAsync(vm.CurrentJson());
        vm.Status = "Copied palette JSON to the clipboard.";
    }

    private async void OnImport(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DesignerViewModel vm) return;
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null) return;
        var text = await clipboard.TryGetTextAsync();
        if (!string.IsNullOrWhiteSpace(text)) vm.ApplyImportedJson(text);
        else vm.Status = "Clipboard has no text to import.";
    }
}
