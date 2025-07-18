using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Text;
using SlopperEditor.UI;
using SlopperEngine.UI.Interaction;
using SlopperEditor.Inspector;

namespace SlopperEditor.Hierarchy;

public class HierarchyObject : UIElement
{
    public readonly SceneObject RepresentedObject;

    readonly SceneObject _inspectorParent;
    InspectorWindow? _inspector;

    public HierarchyObject(SceneObject representedObject) : base(default)
    {
        RepresentedObject = representedObject;
        Children.Add(_inspectorParent = new());

        Layout.Value = DefaultLayouts.DefaultVertical;

        TextButton butt = new(representedObject.GetType().Name ?? "Nameless");
        butt.OnButtonPressed += _ =>
        {
            if (_inspector != null)
                return;

            _inspectorParent.Children.Add(_inspector = new(RepresentedObject));
        };
        UIChildren.Add(butt);

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