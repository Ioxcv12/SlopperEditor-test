using SlopperEngine.UI.Base;
using SlopperEngine.UI.Layout;

namespace SlopperEditor.UI;

/// <summary>
/// A number of easily accessible layouts.
/// </summary>
public static class DefaultLayouts
{
    public static LinearArrangedLayout DefaultVertical =>
        new()
        {
            ChildAlignment = Alignment.Min,
            IsLayoutHorizontal = false,
            StartAtMax = true,
            Padding = UISize.FromPixels(new(5, 2))
        };

    public static LinearArrangedLayout DefaultHorizontal =>
        new()
        {
            ChildAlignment = Alignment.Middle,
            IsLayoutHorizontal = true,
            StartAtMax = false,
            Padding = UISize.FromPixels(new(2))
        };

    public static LinearArrangedLayout PackedVertical =>
        new()
        {
            IsLayoutHorizontal = false,
            StartAtMax = true,
            Padding = default
        };
}