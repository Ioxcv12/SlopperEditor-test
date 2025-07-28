using OpenTK.Windowing.GraphicsLibraryFramework;
using SlopperEditor.UI;
using SlopperEditor.UndoSystem;
using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Interaction;
using SlopperEngine.UI.Layout;
using SlopperEngine.UI.Text;

namespace SlopperEditor.Hierarchy;

/// <summary>
/// A window that can add objects to the scene.
/// </summary>
public class AddObjectWindow : UIElement
{
    public AddObjectWindow(ChildContainer parent, Editor editor) : base(new(0.43f, 0.3f, 0.57f, 0.7f))
    {
        UIChildren.Add(new FloatingWindowHeader(this, "Add new object..."));
        Layout.Value = new LinearArrangedLayout
        {
            Padding = default,
            IsLayoutHorizontal = false,
            StartAtMax = true,
        };

        ScrollableArea content = new();
        UIChildren.Add(content);
        content.Layout.Value = DefaultLayouts.DefaultVertical;

        foreach (var t in ReflectionCache.GetAllSceneObjects().Span)
        {
            if (!ReflectionCache.HasConstructor(t))
            {
                content.UIChildren.Add(new TextBox(t.Name, Style.ForegroundStrong, Style.BackgroundWeak)
                {
                    Scale = 1
                });
                continue;
            }
            var butt = new TextButton(t.Name);
            content.UIChildren.Add(butt);
            butt.OnButtonReleased += mouseButton =>
            {
                if (mouseButton != MouseButton.Left)
                    return;

                if (!ReflectionCache.TryCreate(t, out object? obj))
                {
                    butt.Enabled = false;
                    return;
                }
                var res = (SceneObject)obj;
                var act = new ReparentAction(res, parent);
                if (!parent.TryAdd(res))
                {
                    butt.Enabled = false;
                    res.Destroy();
                    return;
                }
                editor.UndoQueue?.DoAction(act);
                Destroy();
            };
        }
    }

    protected override UIElementSize GetSizeConstraints() => new(Alignment.Middle, Alignment.Middle, 100, 100);
}