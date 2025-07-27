using OpenTK.Mathematics;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Text;

namespace SlopperEditor.Inspector;

public class InspectorName : UIElement
{
    UIElement _value;
    public InspectorName(string name, UIElement value)
    {
        _value = value;
        UIChildren.Add(new TextBox(name, Style.Tint)
        {
            Horizontal = Alignment.Max,
            Scale = 1
        });
    }

    protected override UIElementSize GetSizeConstraints()
    {
        Box2 combined = new(
            Vector2.ComponentMin(_value.LastGlobalShape.Min, _value.LastChildrenBounds.Min),
            Vector2.ComponentMax(_value.LastGlobalShape.Max, _value.LastChildrenBounds.Max));
        Vector2 pixSize = combined.Size / LastRenderer!.GetPixelScale();
        return new(default, default, (int)pixSize.X, (int)pixSize.Y);
    }
}