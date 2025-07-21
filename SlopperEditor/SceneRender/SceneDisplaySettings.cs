using SlopperEditor.UI;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Interaction;

namespace SlopperEditor.SceneRender;

public class SceneDisplaySettings : UIElement
{
    ScrollableArea _area;

    public SceneDisplaySettings(SceneDisplay owner) : base(new(0.8f, 0.7f, 1, 0.9f))
    {
        UIChildren.Add(new FloatingWindowHeader(this, "Scene rendering settings", false));
        UIChildren.Add(_area = new());
        Layout.Value = DefaultLayouts.PackedVertical;
        _area.Layout.Value = DefaultLayouts.DefaultVertical;
    }
    
    protected override UIElementSize GetSizeConstraints() => new(Alignment.Middle, Alignment.Middle, 100, 100);
}