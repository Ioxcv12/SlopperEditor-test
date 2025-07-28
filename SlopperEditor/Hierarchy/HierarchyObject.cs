using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEditor.UI;
using SlopperEngine.UI.Interaction;
using SlopperEditor.Inspector;

namespace SlopperEditor.Hierarchy;

/// <summary>
/// Represents a SceneObject in the hierarchy window.
/// </summary>
public class HierarchyObject : UIElement
{
    /// <summary>
    /// The SceneObject that is represented.
    /// </summary>
    public readonly SceneObject RepresentedObject;

    InspectorWindow? _inspector;

    public HierarchyObject(SceneObject representedObject, Editor editor) : base(default)
    {
        RepresentedObject = representedObject;

        Layout.Value = DefaultLayouts.DefaultVertical;

        TextButton butt = new(representedObject.GetType().Name ?? "Nameless");
        butt.OnButtonPressed += _ =>
        {
            if (_inspector != null)
            {
                editor.FloatingWindowHolder.UIChildren.Add(_inspector);
                return;
            }

            editor.FloatingWindowHolder.UIChildren.Add(_inspector = new(RepresentedObject, editor));
        };
        UIChildren.Add(butt);

        foreach (var childContainer in ReflectionCache.GetChildContainers(representedObject.GetType()))
        {
            if (childContainer.GetValue(representedObject) is not ChildContainer cont)
                continue;
            UIChildren.Add(new HierarchyChildContainer(cont, editor, childContainer.Name));
        }
    }

    /// <summary>
    /// Should be called every time the scene changes.
    /// </summary>
    public void Update()
    {
        foreach (var ch in UIChildren.All)
        {
            if (ch is not HierarchyChildContainer childContainer)
                continue;

            childContainer.Update();
        }
    }

    protected override UIElementSize GetSizeConstraints() => new(Alignment.Max, Alignment.Min, 16, 16);
}