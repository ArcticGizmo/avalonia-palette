using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Palette.Sample.ViewModels;

namespace Palette.Sample;

/// <summary>
/// Resolves a view for a view-model by naming convention: <c>…ViewModels.FooViewModel</c>
/// maps to <c>…Views.FooView</c>. The same pattern the author's apps use.
/// </summary>
public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        if (data is null)
            return new TextBlock { Text = "null" };

        var name = data.GetType().FullName!
            .Replace("ViewModels", "Views", StringComparison.Ordinal)
            .Replace("ViewModel", "View", StringComparison.Ordinal);

        var type = Type.GetType(name);
        if (type is null)
            return new TextBlock { Text = $"View not found: {name}" };

        return (Control)Activator.CreateInstance(type)!;
    }

    public bool Match(object? data) => data is PageViewModel;
}
