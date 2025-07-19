using SlopperEngine.UI.Base;
using SlopperEngine.UI.Display;
using SlopperEngine.UI.Text;

namespace SlopperEditor.UI;

public sealed class FloatingWindowHeader : UIElement
{
    public FloatingWindowHeader(UIElement parent, string title, bool addCloseButton = true) : base(new(0, 1, 1, 1))
    {
        UIChildren.Add(new ColorRectangle(new(0, 0, 1, 1), Style.ForegroundWeak));
        UIChildren.Add(new TextBox(title, Style.Tint, default)
        {
            LocalShape = new(0.5f, 0.5f, 0.5f, 0.5f),
            Scale = 1,
            Horizontal = Alignment.Middle,
            Vertical = Alignment.Middle,
        });
        var dragHitbox = new DragElementHitbox(parent);
        dragHitbox.OnDragStart += () => parent.ParentContainer?.TryAdd(parent);
        UIChildren.Add(dragHitbox);
        if (addCloseButton)
        {
            var butt = TexturedButtons.CreateCloseButton();
            butt.Vertical = Alignment.Middle;
            butt.LocalShape = new(0,0.5f,0,0.5f);
            butt.OnToggle += _ => parent.Destroy();
            UIChildren.Add(butt);
        }
    }

    protected override UIElementSize GetSizeConstraints() => new(default, Alignment.Min, 1, 25);
}