using SlopperEngine.UI.Base;
using SlopperEngine.UI.Layout;

namespace SlopperEditor;

public static class DefaultLayouts
{
    public static LayoutHandler DefaultVertical =>
        new LinearArrangedLayout()
        {
            ChildAlignment = Alignment.Min,
            IsLayoutHorizontal = false,
            StartAtMax = true,
            Padding = UISize.FromPixels(new(5,2))
        };
    
    public static LayoutHandler DefaultHorizontal =>
        new LinearArrangedLayout()
        {
            ChildAlignment = Alignment.Middle,
            IsLayoutHorizontal = true,
            StartAtMax = false,
            Padding = UISize.FromPixels(new(2))
        };
}