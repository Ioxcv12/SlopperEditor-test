using SlopperEngine.UI.Base;
using SlopperEngine.UI.Interaction;
using SlopperEngine.UI.Layout;
using SlopperEngine.UI.Text;
using SlopperEditor.UI;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SlopperEditor.AddObject;
using SlopperEngine.SceneObjects;

namespace SlopperEditor.Hierarchy;

public class HierarchyChildContainer : UIElement
{
    public readonly ChildContainer RepresentedContainer;

    readonly Editor _editor;
    readonly ToggleButton _showToggle;
    int _currentCount;
    AddObjectWindow? _currentOpen;

    public HierarchyChildContainer(ChildContainer representedContainer, Editor editor, string name) : base(default)
    {
        _editor = editor;
        RepresentedContainer = representedContainer;

        Layout.Value = DefaultLayouts.DefaultVertical;

        var header = new Spacer
        {
            MinHeight = 16,
            LocalShape = new(0, 1, 1, 1),
        };
        header.Layout.Value = DefaultLayouts.DefaultHorizontal;
        UIChildren.Add(header);

        header.UIChildren.Add(_showToggle = new());
        _showToggle.OnToggle += ToggleShow;

        var nameButton = new TextButton(name);
        header.UIChildren.Add(nameButton);
        nameButton.OnButtonPressed += mouseButton =>
        {
            if (mouseButton != MouseButton.Left)
                return;

            if (_currentOpen != null)
                return;
            
            editor.FloatingWindowHolder.UIChildren.Add(_currentOpen = new AddObjectWindow(representedContainer, editor));
        };
    }

    void ToggleShow(bool show)
    {
        if (show)
        {
            int ct = RepresentedContainer.Count;
            for (int i = 0; i < ct; i++)
            {
                var ch = RepresentedContainer.GetByIndex(i);
                UIChildren.Add(new HierarchyObject(ch, _editor));
            }
            _currentCount = ct;
        }
        else
        {
            foreach (var ch in UIChildren.AllOfType<HierarchyObject>())
                ch.Destroy();
            _currentCount = 0;
        }
    }

    public void Update()
    {
        if (!_showToggle.Checked)
            return;

        // no swapping position or adding/removing in the same undoqueue update.
        // let me have a LITTLE performance please
        int ct = RepresentedContainer.Count;
        if (_currentCount == ct)
        {
            foreach (var ch in UIChildren.All)
                (ch as HierarchyObject)?.Update();
            return;
        }

        // check for new or removed children
        Dictionary<SceneObject, HierarchyObject> currentChildren = new();
        foreach (var ch in UIChildren.All)
        {
            var child = ch as HierarchyObject;
            if (child == null) continue;
            currentChildren.Add(child.RepresentedObject, child);
            child.Remove();
        }
        for (int i = 0; i < RepresentedContainer.Count; i++)
        {
            var child = RepresentedContainer.GetByIndex(i);
            if (currentChildren.TryGetValue(child, out var rep))
            {
                currentChildren.Remove(child);
                UIChildren.Add(rep);
                rep.Update();
            }
            else
                UIChildren.Add(new HierarchyObject(child, _editor));
        }
        foreach (var leftOver in currentChildren.Values)
            leftOver.Destroy();
    }

    protected override UIElementSize GetSizeConstraints() => new(Alignment.Max, Alignment.Min, 16, 16);
}