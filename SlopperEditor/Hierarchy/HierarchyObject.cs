using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Text;
using SlopperEditor.UI;

namespace SlopperEditor.Hierarchy;

public class HierarchyObject : UIElement
{
    public readonly SceneObject RepresentedObject;

    public HierarchyObject(SceneObject representedObject) : base(default)
    {
        RepresentedObject = representedObject;

        Layout.Value = DefaultLayouts.DefaultVertical;

        UIChildren.Add(new TextBox(representedObject.GetType().Name ?? "Nameless", Style.Tint, Style.BackgroundWeak) { Scale = 1 });

        foreach (var childContainer in ReflectionCache.GetChildContainers(representedObject.GetType()))
        {
            if (childContainer.GetValue(representedObject) is not ChildContainer cont)
                continue;
            UIChildren.Add(new HierarchyChildContainer(cont, childContainer.Name));
        }
    }

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